using Civ2.Rules;
using Civ2engine;
using Model;

namespace Civ2.Dialogs.NewGame.CustomWorldDialogs;

public class CustomiseLandform : SimpleSettingsDialog
{
    public const string Title = "CUSTOMFORM";

    public CustomiseLandform() : base(Title, CustomClimate.Title)
    {
    }

    protected override void SetConfigValue(DialogResult result, PopupBox popupBox)
    {
        Initialization.ConfigObject.Landform = result.SelectedButton == popupBox.Button[0]
            ? Initialization.ConfigObject.Random.Next(popupBox.Options.Count)
            : result.SelectedIndex;
    }
}