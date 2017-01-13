using RestSharp;
using SoLoud.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SoLoud.Helpers
{
    public class Facebook
    {
        public enum Reaction
        {
            LIKE, LOVE, HAHA, WOW, SAD, ANGRY
        }

        public class ReactionPerPerson
        {
            public string id { get; set; }
            public string name { get; set; }
            public Reaction type { get; set; }
        }
        public class ResponsePaging
        {
            public string next { get; set; }
            public string previous { get; set; }
            public Cursors cursors { get; set; }
        }
        public class Cursors
        {
            public string before { get; set; }
            public string after { get; set; }
        }
        public class Summary
        {
            public int total_count { get; set; }
            public string viewer_reaction { get; set; }
        }
        public class PostReactions
        {
            public List<ReactionPerPerson> data { get; set; }
            public ResponsePaging paging { get; set; }
            public Summary summary { get; set; }

        }
        public class FacebookPhotoResponse
        {
            public string id { get; set; }
            public string post_id { get; set; }
        }

        public class Me
        {
            public string id { get; set; }
            public string birthday { get; set; }
            public string email { get; set; }
            public string gedner { get; set; }
            public string name { get; set; }
            public Picture picture { get; set; }
        }

        public class Picture
        {
            public data data { get; set; }
        }

        public class data
        {
            public bool is_silhouette { get; set; }
            public string url { get; set; }
        }

        public class Client
        {
            private string GraphApiURL = "https://graph.facebook.com/v2.8";
            private RestClient RestClient { get; set; }
            private string AccessToken { get; set; }

            public Client(string AccessToken)
            {
                this.AccessToken = AccessToken;
                this.RestClient = new RestClient(GraphApiURL);
            }

            private void handleIfUnsuccessful(IRestResponse resp)
            {
                if (resp.ErrorException != null)
                    throw new HttpException("Something went wrong during HTTP request check inner exception for details", resp.ErrorException);
                else if ((int)resp.StatusCode < 200 || (int)resp.StatusCode >= 400)
                    throw new HttpException((int)resp.StatusCode, resp.ErrorMessage);
            }

            private string Execute(RestRequest req)
            {
                IRestResponse resp = RestClient.Execute(req);
                handleIfUnsuccessful(resp);

                return resp.Content;
            }

            private T Execute<T>(RestRequest req) where T : new()
            {
                IRestResponse<T> resp = RestClient.Execute<T>(req);
                handleIfUnsuccessful(resp);

                return resp.Data;
            }

            private async Task<T> ExecuteAsync<T>(RestRequest req) where T : new()
            {
                IRestResponse<T> resp = await RestClient.ExecuteTaskAsync<T>(req);
                handleIfUnsuccessful(resp);

                return resp.Data;
            }

            public async Task<FacebookPhotoResponse> UploadUserPhoto(Models.File image, bool uploadAsPublished)
            {
                var req = new RestRequest("me/photos", Method.POST);

                req.AddParameter("access_token", AccessToken);
                req.AddParameter("published", uploadAsPublished);
                req.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                req.AddFile("source", image.Content, image.FileName, image.ContentType);

                IRestResponse<FacebookPhotoResponse> resp = await RestClient.ExecuteTaskAsync<FacebookPhotoResponse>(req);

                return resp.Data;
            }

            public async Task<FacebookPhotoResponse[]> UploadUserPhotos(List<Models.File> images, bool uploadAsPublished)
            {
                List<Task<FacebookPhotoResponse>> Tasks = new List<Task<FacebookPhotoResponse>>();

                foreach (var image in images)
                    Tasks.Add(UploadUserPhoto(image, uploadAsPublished));

                FacebookPhotoResponse[] PhotoResponses = await Task.WhenAll(Tasks);

                return PhotoResponses;
            }

            public async Task MultiphotoStory(List<Models.File> images, string Message)
            {
                FacebookPhotoResponse[] PhotoResponses = await UploadUserPhotos(images, false);

                var req = new RestRequest("me/feed", Method.POST);

                req.AddParameter("message", Message);
                req.AddParameter("access_token", AccessToken);

                for (int i = 0; i < PhotoResponses.Length; i++)
                    if (!String.IsNullOrWhiteSpace(PhotoResponses[i].id))
                        req.AddParameter("attached_media[" + i.ToString() + "]", String.Format("{{ \"media_fbid\":\"{0}\"}}", PhotoResponses[i].id));

                Execute(req);
            }
        }
    }
}