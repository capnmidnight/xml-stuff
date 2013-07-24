using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlEdit;

namespace Test.XmlEdit
{
    [TestClass]
    public class Expression__test
    {
        [TestMethod]
        public void CreateExpression_1()
        {
            Expression exp = new Expression("/root/a/b/c");
            Assert.AreEqual("/root/a/b/c", exp.BaseXPath);
            Assert.IsFalse(exp.IsAttribute);
            Assert.IsTrue(exp.IsFromRoot);
            Assert.AreEqual("c", exp.LastStep.ToString(false));
            Assert.AreEqual("root/a/b", exp.Parent.ToString(false));
            Assert.AreEqual(4, exp.StepCount);
            Assert.AreEqual("root", exp[0].ToString(false));
            Assert.AreEqual("a", exp[1].ToString(false));
            Assert.AreEqual("b", exp[2].ToString(false));
            Assert.AreEqual("c", exp[3].ToString(false));
        }
        [TestMethod]
        public void CreateExpression_2()
        {
            Expression exp = new Expression("/root/a/b/c/@d");
            Assert.AreEqual("/root/a/b/c/@d", exp.BaseXPath);
            Assert.IsTrue(exp.IsAttribute);
            Assert.IsTrue(exp.IsFromRoot);
            Assert.AreEqual("@d", exp.LastStep.ToString(false));
            Assert.AreEqual("root/a/b/c", exp.Parent.ToString(false));
            Assert.AreEqual(5, exp.StepCount);
            Assert.AreEqual("root", exp[0].ToString(false));
            Assert.AreEqual("a", exp[1].ToString(false));
            Assert.AreEqual("b", exp[2].ToString(false));
            Assert.AreEqual("c", exp[3].ToString(false));
            Assert.AreEqual("@d", exp[4].ToString(false));
        }
        [TestMethod]
        public void Expression_ExtendedSyntax_ForDescendant()
        {
            Expression exp = new Expression("/root//a");
            Assert.AreEqual("/child::root/descendant::a", exp.ToString(true));
            Assert.AreEqual("/root//a", exp.ToString(false));
        }
        [TestMethod]
        public void Expression_WithElementIndex()
        {
            Expression exp = new Expression("/root/a[0]");
            Assert.AreEqual("/root/a", exp.BaseXPath);
            Assert.AreEqual("/root/a[0]", exp.ToString(false));
            Assert.AreEqual("/child::root/child::a[position()='0']", exp.ToString(true));
        }
        [TestMethod]
        public void SubExpression_1()
        {
            Expression exp = new Expression("/root/a/b");
            Expression e1 = exp.SubExpression(1);
            Assert.AreEqual("/root", e1.ToString(false));
            e1 = exp.SubExpression(2);
            Assert.AreEqual("/root/a", e1.ToString(false));
            e1 = exp.SubExpression(3);
            Assert.AreEqual("/root/a/b", e1.ToString(false));

            e1 = exp.SubExpression(1, 1);
            Assert.AreEqual("a", e1.ToString(false));
            e1 = exp.SubExpression(1, 2);
            Assert.AreEqual("a/b", e1.ToString(false));
            e1 = exp.SubExpression(2, 1);
            Assert.AreEqual("b", e1.ToString(false));
        }

        [TestMethod]
        public void FindCommonBranchPoint_good()
        {
            Expression exp1 = new Expression("/root/a/b/c");
            Expression exp2 = new Expression("/root/a/d/e");
            int bp = exp1.FindCommonBranchIndex(exp2);
            Assert.AreEqual(2, bp);
            Assert.AreEqual("/root/a", exp1.SubExpression(bp).ToString(false));
        }


        [TestMethod]
        public void FindCommonBranchPoint_bad()
        {
            Expression exp1 = new Expression("/root/a/b/c");
            Expression exp2 = new Expression("/not/a/d/e");
            int bp = exp1.FindCommonBranchIndex(exp2);
            Assert.AreEqual(0, bp);
        }

        [TestMethod]
        public void SetNamespacePrefix()
        {
            Expression exp = new Expression("/root/a/b");
            exp.SetNamespacePrefix("ns0");
            Assert.AreEqual("/ns0:root/ns0:a/ns0:b", exp.ToString(false));
            Assert.AreEqual("/child::ns0:root/child::ns0:a/child::ns0:b", exp.ToString(true));
        }

        [TestMethod]
        public void ConstructWithNamespacePrefix()
        {
            Expression exp = new Expression("/root/a/b", "ns0");
            Assert.AreEqual("/ns0:root/ns0:a/ns0:b", exp.ToString(false));
            Assert.AreEqual("/child::ns0:root/child::ns0:a/child::ns0:b", exp.ToString(true));
        }

        [TestMethod]
        public void AddLocationStep()
        {
            Expression exp = new Expression("/root/a/b");
            exp.AddStep(new LocationStep("c"));
            Assert.AreEqual("/root/a/b/c", exp.ToString(false));
        }

        [TestMethod]
        public void AddingStepAfterSettingNamespaceRemembersNamespace()
        {
            Expression exp = new Expression("/root/a/b", "ns0");
            exp.AddStep(new LocationStep("c"));
            Assert.AreEqual("/ns0:root/ns0:a/ns0:b/ns0:c", exp.ToString(false));
        }

        [TestMethod]
        public void AddStepFromString()
        {
            Expression exp = new Expression("/root/a/b");
            exp.AddStep("c");
            Assert.AreEqual("/root/a/b/c", exp.ToString(false));
        }

        [TestMethod]
        public void FigureOutNamespacePrefix()
        {
            Expression exp = new Expression("/ns0:root/ns0:a/ns0:b");
            exp.AddStep("c");
            Assert.AreEqual("/ns0:root/ns0:a/ns0:b/ns0:c", exp.ToString(false));
        }

        [TestMethod]
        public void FigureOutNamespaceFailsSilentyOnAmbigousNamespace()
        {
            Expression exp = new Expression("/ns0:root/ns1:a");
            exp.AddStep("b");
            Assert.AreEqual("/ns0:root/ns1:a/b", exp.ToString(false));
        }

        [TestMethod]
        public void Clone()
        {
            Expression exp1 = new Expression("/root/a/b");
            Expression exp2 = (Expression)exp1.Clone();
            Assert.AreNotSame(exp1, exp2);
            Assert.AreEqual(exp1.ToString(true), exp2.ToString(true));
        }

        [TestMethod]
        public void IsEnumerable()
        {
            Expression exp = new Expression("/root/a/b");
            string check = "";
            foreach (LocationStep step in exp)
            {
                check += string.Format("/{0}", step.ToString(false));
            }
            Assert.AreEqual(exp.ToString(false), check);
        }

        //[TestMethod]
        //public void MakeCommonParentExpression()
        //{
        //    Expression exp1 = new Expression("/root/a[1]/b");
        //    Expression exp2 = new Expression("/root/a/c/d");
        //    Expression common = exp1.MakeCommonParentExpression(exp2);
        //    Assert.AreEqual("/root/a", common.ToString());
        //}

        //[TestMethod]
        //public void MakeRelativeExpression()
        //{
        //    Expression exp1 = new Expression("/root/a/b/c/d");
        //    Expression exp2 = new Expression("/root/a");
        //    Expression common = exp1.MakeRelativeExpression(exp2);
        //    Assert.AreEqual("b/c/d", common.ToString());
        //}

        [TestMethod]
        public void MakeWithParentSpecification()
        {
            string raw = "/root/a/../b";
            Expression exp = new Expression(raw);
            Assert.AreEqual(raw, exp.ToString(false));
            Assert.AreEqual("/child::root/child::a/parent::*/child::b", exp.ToString(true));
        }

        [TestMethod]
        public void MakeFunky()
        {
            string raw = "/root//a[3]/../b";
            Expression exp = new Expression(raw);
            Assert.AreEqual(raw, exp.ToString(false));
            Assert.AreEqual("/child::root/descendant::a[position()='3']/parent::*/child::b", exp.ToString(true));
        }

        [TestMethod]
        public void HasLocationStep()
        {
            Expression xpath = new Expression("/root/a/b/c");
            Assert.IsTrue(xpath.ContainsNodeTest("root"));
            Assert.IsTrue(xpath.ContainsNodeTest("a"));
            Assert.IsTrue(xpath.ContainsNodeTest("b"));
            Assert.IsTrue(xpath.ContainsNodeTest("c"));
        }

        [TestMethod]
        public void GetLocationStepByNodeTest()
        {
            Expression xpath = new Expression("/root/a[1]");
            LocationStep step = xpath.GetLocationStepByNodeTest("a");
            Assert.AreEqual("[1]", step.Predicates[0].ToString());
        }
    }
}
