using NUnit.Framework;
using SoLoud.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework;

namespace SoLoud.Tests
{
    class CustomDataAnnotationTest
    {
        [Test]
        [Category("CustomDataNotation")]
        public void test()
        {
            var cont = new SoLoudContext();

            ApplicationUser asdok = new ApplicationUser();

            cont.Users.Add(asdok);
        }
        [Test]
        [Category("CustomDataNotation")]
        public void TITHAGINEIGAMW()
        {
            Assert.IsTrue(false);
        }
    }
}
