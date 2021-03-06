﻿using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;

namespace CheatSheet.Menus
{
	internal class ExtendedCheatMenu : UISlideWindow
	{
		internal static string CSText(string key, string category = "ExtendedCheatMenu") => CheatSheet.CSText(category, key);
		public CheatSheet mod;
		private static UIImage[] buttons = new UIImage[CheatSheet.ButtonTexture.Count];
		private float spacing = 16f;

		public ExtendedCheatMenu(CheatSheet mod)
		{
			buttons = new UIImage[CheatSheet.ButtonTexture.Count];
			this.mod = mod;
			CanMove = true;
			Width = spacing * 2;
			Height = spacing * 2;

			if (CheatSheet.ButtonTexture.Count == 0)
			{
				UILabel none = new UILabel(CSText("NoExtensionCheatModsInstalled"));
				none.Scale = .3f;
				//none.OverridesMouse = false;
				//none.
				//none.MouseInside = (X => false);
				none.Position = new Vector2(spacing, spacing);
				AddChild(none);
				Height = 100;
				Width = 140;
			}

			if (CheatSheet.ButtonTexture.Count > 0)
			{
				int count = CheatSheet.ButtonTexture.Count;

				int cols = (count + 4) / 5;
				int rows = count >= 5 ? 5 : count;

				for (int j = 0; j < CheatSheet.ButtonTexture.Count; j++)
				{
					UIImage button = new UIImage(CheatSheet.ButtonTexture[j]);
					Vector2 position = new Vector2(spacing + 1, spacing + 1);
					button.Scale = 38f / Math.Max(CheatSheet.ButtonTexture[j].Width, CheatSheet.ButtonTexture[j].Height);

					position.X += j / rows * 40;
					position.Y += j % rows * 40;

					if (CheatSheet.ButtonTexture[j].Height > CheatSheet.ButtonTexture[j].Width)
					{
						position.X += (38 - CheatSheet.ButtonTexture[j].Width) / 2;
					}
					else if (CheatSheet.ButtonTexture[j].Height < CheatSheet.ButtonTexture[j].Width)
					{
						position.Y += (38 - CheatSheet.ButtonTexture[j].Height) / 2;
					}

					button.Position = position;
					button.Tag = j;
					button.onLeftClick += button_onLeftClick;
					button.onHover += button_onHover;
					//	button.ForegroundColor = RecipeBrowser.buttonColor;
					//	uIImage2.Tooltip = RecipeBrowser.categNames[j];
					buttons[j] = button;
					AddChild(button);
				}

				Width += 40 * cols;
				Height += 40 * rows;
			}

			Texture2D texture = mod.GetTexture("UI/closeButton").Value;
			UIImage uIImage = new UIImage(texture);
			uIImage.Anchor = AnchorPosition.TopRight;
			uIImage.Position = new Vector2(Width - spacing / 2, spacing / 2);
			uIImage.onLeftClick += bClose_onLeftClick;
			AddChild(uIImage);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);

			if (Visible && IsMouseInside())
			{
				Main.LocalPlayer.mouseInterface = true;
				Main.LocalPlayer.cursorItemIconEnabled = false;
			}

			float x = FontAssets.MouseText.Value.MeasureString(HoverText).X;
			Vector2 vector = new Vector2(Main.mouseX, Main.mouseY) + new Vector2(16f);
			if (vector.Y > Main.screenHeight - 30)
			{
				vector.Y = Main.screenHeight - 30;
			}

			if (vector.X > Main.screenWidth - x)
			{
				vector.X = Main.screenWidth - 460;
			}

			Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, HoverText, vector.X, vector.Y, new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), Color.Black, Vector2.Zero);
		}

		public override void Update()
		{
			base.Update();
		}

		private void bClose_onLeftClick(object sender, EventArgs e)
		{
			Hide();
			mod.hotbar.DisableAllWindows();
			//base.Visible = false;
		}

		private void button_onLeftClick(object sender, EventArgs e)
		{
			UIImage uIImage = (UIImage)sender;
			int num = (int)uIImage.Tag;

			CheatSheet.ButtonClicked[num]();
		}

		private void button_onHover(object sender, EventArgs e)
		{
			UIImage uIImage = (UIImage)sender;
			int num = (int)uIImage.Tag;

			uIImage.Tooltip = CheatSheet.ButtonTooltip[num]();
		}
	}
}