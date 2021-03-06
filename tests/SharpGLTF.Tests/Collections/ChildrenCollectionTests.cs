﻿using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace SharpGLTF.Collections
{
    [TestFixture]
    [Category("Core")]
    public class ChildrenCollectionTests
    {
        class TestChild : IChildOf<ChildrenCollectionTests>
        {
            public ChildrenCollectionTests LogicalParent { get; private set; }

            public void _SetLogicalParent(ChildrenCollectionTests parent)
            {
                LogicalParent = parent;
            }
        }

        [Test]
        public void TestChildCollectionList1()
        {
            var list = new ChildrenCollection<TestChild, ChildrenCollectionTests>(this);

            Assert.Throws<ArgumentNullException>(() => list.Add(null));

            var item1 = new TestChild();
            Assert.IsNull(item1.LogicalParent);

            list.Add(item1);
            Assert.AreSame(item1.LogicalParent, this);

            Assert.Throws<ArgumentException>(() => list.Add(item1));

            list.Remove(item1);
            Assert.IsNull(item1.LogicalParent);
        }

    }
}
