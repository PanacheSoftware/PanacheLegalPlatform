using Blazorise;

namespace PanacheSoftware.UI.Client.Blazor.Helpers
{
    public static class ColorHelper
    {
        public static Color GetAlertColor(string AlertType)
        {
            switch (AlertType)
            {
                case "Success":
                    return Color.Success;
                case "Danger":
                    return Color.Danger;
                default:
                    break;
            }
            return Color.Info;
        }
    }
}
