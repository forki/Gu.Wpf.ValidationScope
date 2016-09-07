namespace Gu.Wpf.ValidationScope.Ui.Tests
{
    using System.Collections.Generic;

    using Gu.Wpf.ValidationScope.Demo;
    using NUnit.Framework;

    using TestStack.White.UIItems;

    public class ScopeIsErrorScopeWindowTests : WindowTests
    {
        protected override string WindowName { get; } = "ScopeIsErrorScopeWindow";

        public TextBox TextBox => this.Window.Get<TextBox>("TextBox");

        public CheckBox HasErrorCheckBox => this.Window.Get<CheckBox>("HasErrorCheckBox");

        public GroupBox Scope => this.Window.GetByText<GroupBox>("Scope");

        public IReadOnlyList<string> ScopeErrors => this.Scope.GetErrors();

        public string ScopeHasError => this.Scope.Get<Label>("HasErrorTextBlock").Text;

        public GroupBox Node => this.Window.GetByText<GroupBox>("Node");

        public string ChildCount => this.Node.Get<Label>("ChildCountTextBlock").Text;

        public IReadOnlyList<string> NodeErrors => this.Node.GetErrors();

        public string NodeHasError => this.Node.Get<Label>("HasErrorTextBlock").Text;

        public string NodeType => this.Node.Get<Label>("NodeTypeTextBlock").Text;

        [SetUp]
        public void SetUp()
        {
            this.TextBox.Enter("0");
            this.HasErrorCheckBox.Checked = false;
            this.PressTab();
        }

        [Test]
        public void CheckNodeType()
        {
            Assert.AreEqual("Gu.Wpf.ValidationScope.InputNode", this.NodeType);
            this.TextBox.Enter('a');
            Assert.AreEqual("Gu.Wpf.ValidationScope.InputNode", this.NodeType);
            this.TextBox.Enter('1');
            Assert.AreEqual("Gu.Wpf.ValidationScope.InputNode", this.NodeType);
        }

        [Test]
        public void AddTextBoxErrorThenNotifyErrorThenRemoveNotifyThenRemoveTextBoxError()
        {
            Assert.AreEqual("HasError: False", this.ScopeHasError);
            CollectionAssert.IsEmpty(this.ScopeErrors);

            Assert.AreEqual("Children: 0", this.ChildCount);
            Assert.AreEqual("HasError: False", this.NodeHasError);
            CollectionAssert.IsEmpty(this.NodeErrors);
            Assert.AreEqual("Gu.Wpf.ValidationScope.InputNode", this.NodeType);

            this.TextBox.Enter('a');
            var expectedErrors = new[] { "Value 'a' could not be converted." };
            Assert.AreEqual("HasError: True", this.ScopeHasError);
            CollectionAssert.AreEqual(expectedErrors, this.ScopeErrors);

            Assert.AreEqual("Children: 0", this.ChildCount);
            Assert.AreEqual("HasError: True", this.NodeHasError);
            CollectionAssert.AreEqual(expectedErrors, this.NodeErrors);
            Assert.AreEqual("Gu.Wpf.ValidationScope.InputNode", this.NodeType);

            this.HasErrorCheckBox.Checked = true;
            expectedErrors = new[] { "Value 'a' could not be converted.", "INotifyDataErrorInfo error" };
            Assert.AreEqual("HasError: True", this.ScopeHasError);
            CollectionAssert.AreEqual(expectedErrors, this.ScopeErrors);

            Assert.AreEqual("Children: 0", this.ChildCount);
            Assert.AreEqual("HasError: True", this.NodeHasError);
            CollectionAssert.AreEqual(expectedErrors, this.NodeErrors);
            Assert.AreEqual("Gu.Wpf.ValidationScope.InputNode", this.NodeType);

            this.HasErrorCheckBox.Checked = false;
            this.Window.WaitWhileBusy();
            Assert.AreEqual("HasError: True", this.ScopeHasError);
            CollectionAssert.AreEqual(expectedErrors, this.ScopeErrors);

            Assert.AreEqual("Children: 0", this.ChildCount);
            Assert.AreEqual("HasError: True", this.NodeHasError);
            CollectionAssert.AreEqual(expectedErrors, this.NodeErrors);
            Assert.AreEqual("Gu.Wpf.ValidationScope.InputNode", this.NodeType);

            this.TextBox.Enter('1');
            Assert.AreEqual("HasError: False", this.ScopeHasError);
            CollectionAssert.IsEmpty(this.ScopeErrors);

            Assert.AreEqual("Children: 0", this.ChildCount);
            Assert.AreEqual("HasError: False", this.NodeHasError);
            CollectionAssert.IsEmpty(this.NodeErrors);
            Assert.AreEqual("Gu.Wpf.ValidationScope.InputNode", this.NodeType);
        }

        [Test]
        public void Updates2()
        {
            var childCountBlock = this.Window.Get<Label>(AutomationIDs.ChildCountTextBlock);

            Assert.AreEqual("Children: 0", childCountBlock.Text);
            CollectionAssert.IsEmpty(this.Window.GetErrors());
            var textBox1 = this.Window.Get<TextBox>(AutomationIDs.TextBox1);
            textBox1.Enter('a');
            Assert.AreEqual("Children: 1", childCountBlock.Text);
            CollectionAssert.AreEqual(new[] { "Value 'a' could not be converted." }, this.Window.GetErrors());

            var hasErrorBox = this.Window.Get<CheckBox>(AutomationIDs.HasErrorsBox);
            hasErrorBox.Checked = true;
            var expectedErrors = new[] { "Value 'a' could not be converted.", "INotifyDataErrorInfo error" };
            Assert.AreEqual("Children: 1", childCountBlock.Text);
            CollectionAssert.AreEqual(expectedErrors, this.Window.GetErrors());

            textBox1.Enter('1');
            expectedErrors = new[] { "INotifyDataErrorInfo error" };
            Assert.AreEqual("Children: 0", childCountBlock.Text);
            CollectionAssert.AreEqual(expectedErrors, this.Window.GetErrors());

            hasErrorBox.Checked = false;
            Assert.AreEqual("Children: 0", childCountBlock.Text);
            CollectionAssert.IsEmpty(this.Window.GetErrors());
        }
    }
}