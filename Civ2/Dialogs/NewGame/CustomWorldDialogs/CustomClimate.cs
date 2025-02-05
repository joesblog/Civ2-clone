using Civ2.Rules;
using Civ2engine;
using Model;

namespace Civ2.Dialogs.NewGame.CustomWorldDialogs;

public class CustomClimate: SimpleSettingsDialog
{
    public const string Title = "CUSTOMCLIMATE";

    public CustomClimate() : base(Title, CustomTemp.Title)
    {
    }

    protected override void SetConfigValue(DialogResult result, PopupBox popupBox)
    {
        Initialization.ConfigObject.Climate = result.SelectedButton == popupBox.Button[0]
            ? Initialization.ConfigObject.Random.Next(popupBox.Options.Count)
            : result.SelectedIndex;
    }
}