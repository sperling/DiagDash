using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using ICSharpCode.ILSpy.XmlDoc;

namespace DiagDash.Test
{
    [TestClass]
    public class CodeDocTests
    {
        [TestMethod]
        public void CanGetDocForHttpRuntimeAppDomainAppId()
        {
            var type = typeof(HttpRuntime);
            var member = type.GetMember("AppDomainAppId")[0];
            var docProvider = XmlDocLoader.LoadDocumentation(member.Module);
            Assert.IsNotNull(docProvider);
            string documentation = docProvider.GetDocumentation(XmlDocKeyProvider.GetKey(member));
            Assert.IsFalse(String.IsNullOrEmpty(documentation));
        }

        [TestMethod]
        public void CanRenderDocForHttpRuntimeAppDomainAppId()
        {
            var type = typeof(HttpRuntime);
            var member = type.GetMember("AppDomainAppId")[0];
            var docProvider = XmlDocLoader.LoadDocumentation(member.Module);
            Assert.IsNotNull(docProvider);
            string documentation = docProvider.GetDocumentation(XmlDocKeyProvider.GetKey(member));
            Assert.IsFalse(String.IsNullOrEmpty(documentation));
            var renderer = new XmlDocRenderer();
            renderer.AddXmlDocumentation(documentation);
            var renderedDoc = renderer.CreateTextBlock();
            Assert.IsFalse(String.IsNullOrEmpty(renderedDoc));
        }
    }
}
