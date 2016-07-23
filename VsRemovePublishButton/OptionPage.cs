namespace VsRemovePublishButton {
    using System.Windows.Forms;

    public partial class OptionPage : UserControl {
        public OptionPage(OptionPageCustom optionsHolder) {
            this.InitializeComponent();

            this.OptionsHolder = optionsHolder;

            this.chkPublishButton.Checked = this.OptionsHolder.HideByDefault;
        }

        internal OptionPageCustom OptionsHolder;

        private void chkPublishButton_OnCheckedChanged(object sender, System.EventArgs e) {
            this.OptionsHolder.HideByDefault = this.chkPublishButton.Checked;
        }

        private void btnToggleVisibility_OnClick(object sender, System.EventArgs e) {
            PublishButton.Toggle();
        }

        public void OnActivate() {
            this.btnToggleVisibility.Visible = PublishButton.IsAvailable();
        }
    }
}