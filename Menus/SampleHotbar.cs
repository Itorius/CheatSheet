using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace CheatSheet.Menus
{
	internal class SampleHotbar : UIHotbar
	{
		public UIView buttonView;
		public UIImage bSampleButton;

		internal bool mouseDown;
		internal bool justMouseDown;

		private CheatSheet mod;

		public SampleHotbar(CheatSheet mod)
		{
			this.mod = mod;
			//parentHotbar = mod.hotbar;

			buttonView = new UIView();
			Visible = false;

			// Button images
			bSampleButton = new UIImage(TextureAssets.Item[ItemID.Paintbrush].Value);

			// Button tooltips
			bSampleButton.Tooltip = "Sample Tooltip";

			// Button EventHandlers
			bSampleButton.onLeftClick += bSampleButton_onLeftClick;
			bSampleButton.onRightClick += (s, e) =>
			{
				// Sample handling
			};

			// Register mousedown
			onMouseDown += (s, e) =>
			{
				if (!Main.LocalPlayer.mouseInterface && !mod.hotbar.MouseInside && !mod.hotbar.button.MouseInside)
				{
					mouseDown = true;
					Main.LocalPlayer.mouseInterface = true;
				}
			};
			onMouseUp += (s, e) =>
			{
				justMouseDown = true;
				mouseDown = false; /*startTileX = -1; startTileY = -1;*/
			};

			// ButtonView
			buttonView.AddChild(bSampleButton);

			Width = 200f;
			Height = 55f;
			buttonView.Height = Height;
			Anchor = AnchorPosition.Top;
			AddChild(buttonView);
			Position = new Vector2(Hotbar.xPosition, hiddenPosition);
			CenterXAxisToParentCenter();
			float num = spacing;
			for (int i = 0; i < buttonView.children.Count; i++)
			{
				buttonView.children[i].Anchor = AnchorPosition.Left;
				buttonView.children[i].Position = new Vector2(num, 0f);
				buttonView.children[i].CenterYAxisToParentCenter();
				buttonView.children[i].Visible = true;
				buttonView.children[i].ForegroundColor = buttonUnselectedColor;
				num += buttonView.children[i].Width + spacing;
			}

			Resize();
		}

		private void bSampleButton_onLeftClick(object sender, EventArgs e)
		{
			// Sample handling left click
		}

		public override void Update()
		{
			DoSlideMovement();

			CenterXAxisToParentCenter();
			base.Update();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (Visible)
			{
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, _rasterizerState, null, Main.UIScaleMatrix);
				//	Rectangle scissorRectangle = new Rectangle((int)base.X- (int)base.Width, (int)base.Y, (int)base.Width, (int)base.Height);
				//Parent.Position.Y
				//		Main.NewText((int)Parent.Position.Y + " " + (int)shownPosition);
				//	Rectangle scissorRectangle = new Rectangle((int)(base.X - base.Width / 2), (int)(shownPosition), (int)base.Width, (int)base.Height);
				Rectangle scissorRectangle = new Rectangle((int)(X - Width / 2), (int)shownPosition, (int)Width, (int)(mod.hotbar.Position.Y - shownPosition));
				/*if (scissorRectangle.X < 0)
				{
					scissorRectangle.Width += scissorRectangle.X;
					scissorRectangle.X = 0;
				}
				if (scissorRectangle.Y < 0)
				{
					scissorRectangle.Height += scissorRectangle.Y;
					scissorRectangle.Y = 0;
				}
				if ((float)scissorRectangle.X + base.Width > (float)Main.screenWidth)
				{
					scissorRectangle.Width = Main.screenWidth - scissorRectangle.X;
				}
				if ((float)scissorRectangle.Y + base.Height > (float)Main.screenHeight)
				{
					scissorRectangle.Height = Main.screenHeight - scissorRectangle.Y;
				}*/
				scissorRectangle = CheatSheet.GetClippingRectangle(spriteBatch, scissorRectangle);
				Rectangle scissorRectangle2 = spriteBatch.GraphicsDevice.ScissorRectangle;
				spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;

				base.Draw(spriteBatch);

				spriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle2;
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Main.UIScaleMatrix);
			}

			//	base.Draw(spriteBatch);

			if (Visible && base.IsMouseInside())
			{
				Main.LocalPlayer.mouseInterface = true;
				//Main.LocalPlayer.cursorItemIconEnabled = false;
			}

			if (Visible && IsMouseInside())
			{
				Main.LocalPlayer.mouseInterface = true;
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

		protected override bool IsMouseInside()
		{
			return hidden ? false : base.IsMouseInside();
		}

		public void Resize()
		{
			float num = spacing;
			for (int i = 0; i < buttonView.children.Count; i++)
			{
				if (buttonView.children[i].Visible)
				{
					buttonView.children[i].X = num;
					num += buttonView.children[i].Width + spacing;
				}
			}

			Width = num;
			buttonView.Width = Width;
		}

		public void Hide()
		{
			hidden = true;
			arrived = false;
		}

		public void Show()
		{
			mod.hotbar.currentHotbar = this;
			arrived = false;
			hidden = false;
			Visible = true;
		}
	}
}