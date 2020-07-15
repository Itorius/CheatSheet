// using CheatSheet.CustomUI;
// using CheatSheet.UI;
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using System;
// using Terraria;
// using Terraria.GameContent;
// using Terraria.ID;
// using Terraria.ModLoader;
//
// namespace CheatSheet.Menus
// {
// 	internal class ConfigurationTool
// 	{
// 		internal static string CSText(string key, string category = "Hotbar") => CheatSheet.CSText(category, key);
// 		public static UIImage button;
// 		public static ConfigurationWindow configurationWindow;
// 		public static CheatSheet cheatSheet;
//
// 		public static UIImage GetButton(Mod mod)
// 		{
// 			cheatSheet = mod as CheatSheet;
//
// 			configurationWindow = new ConfigurationWindow(cheatSheet);
// 			configurationWindow.SetDefaultPosition(new Vector2(200, 200));
// 			configurationWindow.Visible = false;
//
// 			button = new UIImage(TextureAssets.Item[ItemID.Cog].Value);
// 			button.Tooltip = CSText("ConfigureAvailableTools");
// 			button.onLeftClick += bConfigurationToggle_onLeftClick;
// 			//+= (s, e) =>
// 			//{
// 			//	configurationWindow.selected = true;
// 			//};
// 			button.ForegroundColor = Color.White;
//
// 			return button;
// 		}
//
// 		private static void bConfigurationToggle_onLeftClick(object sender, EventArgs e)
// 		{
// 			UIImage uIImage = (UIImage)sender;
//
// 			if (configurationWindow.selected)
// 			{
// 				configurationWindow.selected = false;
// 				uIImage.ForegroundColor = Hotbar.buttonUnselectedColor;
// 			}
// 			else
// 			{
// 				cheatSheet.hotbar.DisableAllWindows();
// 				configurationWindow.selected = true;
// 				uIImage.ForegroundColor = Hotbar.buttonSelectedColor;
// 			}
// 		}
// 	}
//
// 	internal class ConfigurationWindow : UISlideWindow
// 	{
// 		internal static string CSText(string key, string category = "ConfigurationTool") => CheatSheet.CSText(category, key);
// 		public CheatSheet mod;
// 		private float spacing = 16f;
//
// 		public ConfigurationWindow(CheatSheet mod)
// 		{
// 			this.mod = mod;
// 			CanMove = true;
// 			Width = 280;
// 			Height = 358;
//
// 			Texture2D texture = mod.GetTexture("UI/closeButton").Value;
// 			UIImage uIImage = new UIImage(texture);
// 			uIImage.Anchor = AnchorPosition.TopRight;
// 			uIImage.Position = new Vector2(Width - spacing, spacing);
// 			uIImage.onLeftClick += bClose_onLeftClick;
// 			AddChild(uIImage);
//
// 			//ConfigurationLoader.Initialized();
//
// 			string[] labels =
// 			{
// 				CSText("ItemBrowser"), CSText("NPCBrowser"), CSText("RecipeBrowser"), CSText("MinionBooster"), CSText("Butcher"), CSText("ClearMenu"),
// 				CSText("ExtraAccessorySlots"), CSText("ModExtensions"), CSText("PaintTools"), CSText("SpawnRate"), CSText("Vacuum"), CSText("Waypoints"), CSText("LightHack"), CSText("GodMode")
// 				/* "Boss Downer", "Event Manager"*/
// 			};
// 			Func<bool>[] selecteds =
// 			{
// 				() => ConfigurationLoader.personalConfiguration.ItemBrowser,
// 				() => ConfigurationLoader.personalConfiguration.NPCBrowser,
// 				() => ConfigurationLoader.personalConfiguration.RecipeBrowser,
// 				() => ConfigurationLoader.personalConfiguration.MinionBooster,
// 				() => ConfigurationLoader.personalConfiguration.Butcher,
// 				() => ConfigurationLoader.personalConfiguration.ClearMenu,
// 				() => ConfigurationLoader.personalConfiguration.ExtraAccessorySlots,
// 				() => ConfigurationLoader.personalConfiguration.ModExtensions,
// 				() => ConfigurationLoader.personalConfiguration.PaintTools,
// 				() => ConfigurationLoader.personalConfiguration.SpawnRate,
// 				() => ConfigurationLoader.personalConfiguration.Vacuum,
// 				() => ConfigurationLoader.personalConfiguration.Waypoints,
// 				() => ConfigurationLoader.personalConfiguration.LightHack,
// 				() => ConfigurationLoader.personalConfiguration.GodMode,
// 				//  ConfigurationLoader.configuration.BossDowner,
// 				//  ConfigurationLoader.configuration.EventManager,
// 			};
//
// 			Action<bool>[] assignSelected =
// 			{
// 				a => ConfigurationLoader.personalConfiguration.ItemBrowser = a,
// 				a => ConfigurationLoader.personalConfiguration.NPCBrowser = a,
// 				a => ConfigurationLoader.personalConfiguration.RecipeBrowser = a,
// 				a => ConfigurationLoader.personalConfiguration.MinionBooster = a,
// 				a => ConfigurationLoader.personalConfiguration.Butcher = a,
// 				a => ConfigurationLoader.personalConfiguration.ClearMenu = a,
// 				a => ConfigurationLoader.personalConfiguration.ExtraAccessorySlots = a,
// 				a => ConfigurationLoader.personalConfiguration.ModExtensions = a,
// 				a => ConfigurationLoader.personalConfiguration.PaintTools = a,
// 				a => ConfigurationLoader.personalConfiguration.SpawnRate = a,
// 				a => ConfigurationLoader.personalConfiguration.Vacuum = a,
// 				a => ConfigurationLoader.personalConfiguration.Waypoints = a,
// 				a => ConfigurationLoader.personalConfiguration.LightHack = a,
// 				a => ConfigurationLoader.personalConfiguration.GodMode = a,
// 			};
//
// 			for (int i = 0; i < labels.Length; i++)
// 			{
// 				int iClosure = i;
// 				UICheckbox cb = new UICheckbox(labels[i]);
// 				cb.Selected = selecteds[i]();
// 				cb.X = spacing;
// 				cb.Y = i * 24 + spacing;
// 				//cb.SelectedChanged += new EventHandler(bCheckBoxTicked);
// 				cb.SelectedChanged += (a, b) =>
// 				{
// 					assignSelected[iClosure](cb.Selected);
// 					cb.Selected = selecteds[iClosure]();
// 					ConfigurationLoader.SaveSetting();
// 					mod.hotbar.ChangedConfiguration();
// 					ConfigurationTool.configurationWindow.selected = true;
// 				};
//
// 				//cb.label.ForegroundColor = Color.Red;
// 				AddChild(cb);
// 			}
// 		}
//
// 		private void bClose_onLeftClick(object sender, EventArgs e)
// 		{
// 			Hide();
// 			mod.hotbar.DisableAllWindows();
// 		}
//
// 		public override void Draw(SpriteBatch spriteBatch)
// 		{
// 			base.Draw(spriteBatch);
//
// 			if (Visible && IsMouseInside())
// 			{
// 				Main.LocalPlayer.mouseInterface = true;
// 				Main.LocalPlayer.cursorItemIconEnabled = false;
// 			}
// 		}
// 	}
// }