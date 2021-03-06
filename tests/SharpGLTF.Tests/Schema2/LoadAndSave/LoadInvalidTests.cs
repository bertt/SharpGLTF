﻿using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace SharpGLTF.Schema2.LoadAndSave
{
    [TestFixture]
    [Category("Model Load and Save")]
    public class LoadInvalidTests
    {
        [Test]
        public void LoadInvalidJsonModel()
        {
            var path = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, "Assets", "Invalid_Json.gltf");

            Assert.Throws<Validation.SchemaException>(() => ModelRoot.Load(path));
        }
    }
}
