using HttpUtils;
using Moq;
using NUnit.Framework;
using SoLoud.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SoLoud.Tests
{
    class ContestsControllerTest
    {
        private List<Models.File> GetFileData()
        {
            var returnValue = new List<Models.File>();

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../images/img_7723_0.jpg");
            var Filestream = System.IO.File.Open(path, FileMode.Open);
            returnValue.Add(new Models.File() { Content = Misc.ToByteArray(Filestream), ContentType = MimeMapping.GetMimeMapping("img_7723_0.jpg"), FileName = "img_7723_0.jpg", FileId = 1, FileType = FileType.Photo, ExampleForContestId = "1" });

            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../images/ntinos.png");
            Filestream = System.IO.File.Open(path, FileMode.Open);
            returnValue.Add(new Models.File() { Content = Misc.ToByteArray(Filestream), ContentType = MimeMapping.GetMimeMapping("ntinos.png"), FileName = "ntinos.png", FileId = 1, FileType = FileType.Photo, ProductForContestId = "1" });

            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../images/Miro_Aldrenfar.png");
            Filestream = System.IO.File.Open(path, FileMode.Open);
            returnValue.Add(new Models.File() { Content = Misc.ToByteArray(Filestream), ContentType = MimeMapping.GetMimeMapping("Miro_Aldrenfar.png"), FileName = "Miro_Aldrenfar.png", FileId = 1, FileType = FileType.Photo, PostId = "1" });

            return returnValue;
        }

        private List<Contest> GetContestData()
        {
            var returnValue = new List<Contest>();

            returnValue.Add(new Contest() { Id = "1", Category = Categoies.Charity, Title = "Charitable Contest" });
            returnValue.Add(new Contest() { Id = "2", Category = Categoies.Fashion, Title = "Fashionable Contest" });
            returnValue.Add(new Contest() { Id = "3", Category = Categoies.HomeDecoration, Title = "Food Contest" });

            return returnValue;
        }

        private List<Post> GetPostData()
        {
            var returnValue = new List<Post>();

            returnValue.Add(new Post() { Id = "1", ContestId = "1", HashTags = "potato", Text = "This a post" });
            returnValue.Add(new Post() { Id = "2", ContestId = "2", HashTags = "potato", Text = "This a post number 2" });
            returnValue.Add(new Post() { Id = "3", ContestId = "3", HashTags = "potato", Text = "This a post too" });

            return returnValue;
        }

        private DbSet<T> GetMockDataSet<T>(List<T> Data, Mock<SoLoudContext> MockContext) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(Data.AsQueryable().Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(Data.AsQueryable().Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(Data.AsQueryable().ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(Data.AsQueryable().GetEnumerator());
            mockSet.Setup(m => m.Add(It.IsAny<T>())).Callback<T>(Data.Add);
            mockSet.Setup(m => m.AddRange(It.IsAny<List<T>>())).Callback<IEnumerable<T>>((l) => { Data.AddRange(l); });
            mockSet.Setup(m => m.Remove(It.IsAny<T>())).Callback<T>((s) => { Data.Remove(s); });
            //MockContext.Setup(m => m.Files).Returns(mockSet.Object); //This must be done outside. Couldn't set it dynamicly
            MockContext.Setup(m => m.Set<T>()).Returns(mockSet.Object);

            var mockDBSet = new Mock<DbSet>();
            mockDBSet.As<IQueryable>().Setup(m => m.Provider).Returns(Data.AsQueryable().Provider);
            mockDBSet.As<IQueryable>().Setup(m => m.Expression).Returns(Data.AsQueryable().Expression);
            mockDBSet.As<IQueryable>().Setup(m => m.ElementType).Returns(Data.AsQueryable().ElementType);
            mockDBSet.As<IQueryable>().Setup(m => m.GetEnumerator()).Returns(Data.AsQueryable().GetEnumerator());
            MockContext.Setup(m => m.Set(typeof(T))).Returns((DbSet)mockDBSet.Object);

            return mockSet.Object;
        }

        private Mock<SoLoudContext> GetMockedContext()
        {
            var mockContext = new Mock<SoLoudContext>();

            //Mock Files
            List<Models.File> fileData = GetFileData();
            var mockFileSet = GetMockDataSet<Models.File>(fileData, mockContext);
            mockContext.Setup(m => m.Files).Returns(mockFileSet);

            //Mock Contests
            List<Contest> ContestData = GetContestData();
            var mockContestSet = GetMockDataSet<Contest>(ContestData, mockContext);
            mockContext.Setup(m => m.Contests).Returns(mockContestSet);

            //Mock Posts
            List<Post> PostData = GetPostData();
            var mockPostSet = GetMockDataSet<Post>(PostData, mockContext);
            mockContext.Setup(m => m.Posts).Returns(mockPostSet);

            return mockContext;
        }

        [Test]
        public void test()
        {
            var mockContext = GetMockedContext();
        }
    }
}
