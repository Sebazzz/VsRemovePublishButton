namespace VsRemovePublishButton {
    using System.Windows;
    using System.Windows.Media;

    internal class PublishButton {
        private const string ElementId = "PublishCompartment";


        public static bool TryHide() {
            FrameworkElement publishElement = GetPublishButton();

            if (publishElement != null) {
                publishElement.Visibility = Visibility.Collapsed;
                return true;
            }

            return false;
        }

        public static bool Toggle() {
            FrameworkElement publishElement = GetPublishButton();

            if (publishElement != null) {
                publishElement.Visibility = publishElement.Visibility != Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
                return true;
            }

            return false;
        }

        public static bool IsAvailable() {
            return GetPublishButton() != null;
        }

        private static FrameworkElement GetPublishButton() {
            return FindElement(Application.Current.MainWindow, ElementId);
        }

        private static FrameworkElement FindElement(Visual v, string name) {
            if (v == null) {
                return null;
            }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(v); ++i) {
                var child = VisualTreeHelper.GetChild(v, i) as Visual;

                var e = child as FrameworkElement;
                if (e != null && e.Name == name) {
                    return e;
                }

                var result = FindElement(child, name);
                if (result != null) {
                    return result;
                }
            }

            return null;
        }
    }
}