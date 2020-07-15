using CheatSheet.CustomUI;
using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CheatSheet.Menus
{
	internal enum NPCBrowserCategories
	{
		AllNPCs,
		Bosses,
		TownNPC,
		netID,
		FilteredNPCs,
		ModNPCs
	}

	internal class NPCBrowser : UISlideWindow
	{
		internal static string CSText(string key, string category = "MobBrowser") => CheatSheet.CSText(category, key);
		internal static NPC tooltipNpc;
		internal static NPC hoverNpc;
		internal Texture2D[] textures;

		private static string[] categNames =
		{
			CSText("AllNPCs"),
			CSText("Bosses"),
			CSText("TownNPCs"),
			CSText("NetIDNPCs"),
			CSText("FilteredNPCs"),
			CSText("CycleModSpecificNPCs")
		};

		private bool swapFilter;

		public NPCView npcView;
		public CheatSheet mod;

		//	private static List<string> categoryNames = new List<string>();
		internal static UIImage[] bCategories;

		// these 2 indexed by slot number, not npcid, to account for negative.
		public static Dictionary<string, List<int>> ModToNPCs = new Dictionary<string, List<int>>();
		public static List<List<int>> categories = new List<List<int>>();
		private static Color buttonColor = new Color(190, 190, 190);

		private static Color buttonSelectedColor = new Color(209, 142, 13);

		private UITextbox textbox;

		private float spacing = 16f;

		public int lastModNameNumber;

		private float numWidth = categNames.Length - 5; // when adding more filtering buttons, decreases textbar size

		// filteredNPCSlots represents currently loaded npc.
		public static List<int> filteredNPCSlots = new List<int>();

		internal static bool needsUpdate = true;

		// 270 : 16 40 ?? 16

		public NPCBrowser(CheatSheet mod)
		{
			categories.Clear();
			ModToNPCs.Clear();
			npcView = new NPCView();
			this.mod = mod;
			CanMove = true;
			Width = npcView.Width + spacing * 2f;
			Height = 300f; // 272f
			npcView.Position = new Vector2(spacing, Height - npcView.Height - spacing * 3f);
			AddChild(npcView);
			ParseList2();
			Texture2D texture = mod.GetTexture("UI/closeButton").Value;
			UIImage uIImage = new UIImage(texture);
			uIImage.Anchor = AnchorPosition.TopRight;
			uIImage.Position = new Vector2(Width - spacing, spacing);
			uIImage.onLeftClick += bClose_onLeftClick;
			AddChild(uIImage);
			textbox = new UITextbox();
			textbox.Anchor = AnchorPosition.BottomLeft;
			//this.textbox.Position = new Vector2(base.Width - this.spacing * 2f + uIImage.Width * numWidth * 2, this.spacing /** 2f + uIImage.Height*/);
			textbox.Position = new Vector2(spacing, Height - spacing);
			textbox.KeyPressed += textbox_KeyPressed;
			AddChild(textbox);
			
			Main.instance.LoadItem(ItemID.AlphabetStatueA);
			Main.instance.LoadItem(ItemID.AlphabetStatueB);
			Main.instance.LoadItem(ItemID.AlphabetStatueT);
			Main.instance.LoadItem(ItemID.AlphabetStatueN);
			Main.instance.LoadItem(ItemID.AlphabetStatueF);
			Main.instance.LoadItem(ItemID.AlphabetStatueM);
			
			Texture2D[] categoryIcons =
			{
				TextureAssets.Item[ItemID.AlphabetStatueA].Value,
				TextureAssets.Item[ItemID.AlphabetStatueB].Value,
				TextureAssets.Item[ItemID.AlphabetStatueT].Value,
				TextureAssets.Item[ItemID.AlphabetStatueN].Value,
				TextureAssets.Item[ItemID.AlphabetStatueF].Value,
				TextureAssets.Item[ItemID.AlphabetStatueM].Value,
			};
			
			bCategories = new UIImage[categoryIcons.Length];
			for (int j = 0; j < categoryIcons.Length; j++)
			{
				UIImage uIImage2 = new UIImage(categoryIcons[j]);
				Vector2 position = new Vector2(spacing, spacing);
				uIImage2.Scale = 32f / Math.Max(categoryIcons[j].Width, categoryIcons[j].Height);

				position.X += j % 6 * 40;
				position.Y += j / 6 * 40;

				if (categoryIcons[j].Height > categoryIcons[j].Width)
				{
					position.X += (32 - categoryIcons[j].Width) / 2;
				}
				else if (categoryIcons[j].Height < categoryIcons[j].Width)
				{
					position.Y += (32 - categoryIcons[j].Height) / 2;
				}

				uIImage2.Position = position;
				uIImage2.Tag = j;
				uIImage2.onLeftClick += (s, e) => buttonClick(s, e, true);
				uIImage2.onRightClick += (s, e) => buttonClick(s, e, false);
				uIImage2.ForegroundColor = buttonColor;
				if (j == 0)
				{
					uIImage2.ForegroundColor = buttonSelectedColor;
				}

				uIImage2.Tooltip = categNames[j];
				bCategories[j] = uIImage2;
				AddChild(uIImage2);
			}

			npcView.selectedCategory = categories[0].ToArray();
			npcView.activeSlots = npcView.selectedCategory;
			npcView.ReorderSlots();
			textures = new Texture2D[]
			{
				mod.GetTexture("UI/NPCLifeIcon").Value,
				mod.GetTexture("UI/NPCDamageIcon").Value,
				mod.GetTexture("UI/NPCDefenseIcon").Value,
				mod.GetTexture("UI/NPCKnockbackIcon").Value,
			};
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

			if (hoverNpc != null)
			{
				if (tooltipNpc == null || tooltipNpc.netID != hoverNpc.netID)
				{
					tooltipNpc = new NPC();
					tooltipNpc.SetDefaults(hoverNpc.netID);
				}

				string[] texts = { $"{tooltipNpc.lifeMax}", $"{tooltipNpc.defDamage}", $"{tooltipNpc.defDefense}", $"{tooltipNpc.knockBackResist:0.##}" };
				Vector2 pos = new Vector2(vector.X, vector.Y + 24);
				for (int i = 0; i < textures.Length; i++)
				{
					spriteBatch.Draw(textures[i], pos, Color.White);
					pos.X += textures[i].Width + 4;
					Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, texts[i], pos.X, pos.Y, Color.White, Color.Black, Vector2.Zero);
					pos.X += FontAssets.MouseText.Value.MeasureString(texts[i]).X + 8;
				}
			}
		}

		public override void Update()
		{
			if (needsUpdate)
			{
				foreach (var npcslot in npcView.allNPCSlot)
				{
					npcslot.isFiltered = filteredNPCSlots.Contains(npcslot.netID);
				}

				needsUpdate = false;
			}

			var mouseState = Mouse.GetState();
			MousePrevLeftButton = MouseLeftButton;
			MouseLeftButton = mouseState.LeftButton == ButtonState.Pressed;
			MousePrevRightButton = MouseRightButton;
			MouseRightButton = mouseState.RightButton == ButtonState.Pressed;
			ScrollAmount = PlayerInput.ScrollWheelDeltaForUI;
			//UIView.ScrollAmount = (Main.mouseState.ScrollWheelValue - Main.oldMouseState.ScrollWheelValue) / 2;
			//UIView.HoverItem = UIView.EmptyItem;
			HoverText = "";
			HoverOverridden = false;
			hoverNpc = null;

			base.Update();
		}

		private void bClose_onLeftClick(object sender, EventArgs e)
		{
			Hide();
			mod.hotbar.DisableAllWindows();
			//base.Visible = false;
		}

		private void buttonClick(object sender, EventArgs e, bool left)
		{
			UIImage uIImage = (UIImage)sender;
			int num = (int)uIImage.Tag;
			if (num == (int)NPCBrowserCategories.ModNPCs)
			{
				var mods = ModToNPCs.Keys.ToList();
				mods.Sort();
				if (mods.Count == 0)
				{
					Main.NewText("No NPC have been added by mods.");
				}
				else
				{
					if (uIImage.ForegroundColor == buttonSelectedColor) lastModNameNumber = left ? (lastModNameNumber + 1) % mods.Count : (mods.Count + lastModNameNumber - 1) % mods.Count;
					string currentMod = mods[lastModNameNumber];
					npcView.selectedCategory = categories[0].Where(x => npcView.allNPCSlot[x].npcType >= NPCID.Count && NPCLoader.GetNPC(npcView.allNPCSlot[x].npcType).mod.Name == currentMod).ToArray();
					npcView.activeSlots = npcView.selectedCategory;
					npcView.ReorderSlots();
					bCategories[num].Tooltip = categNames[num] + ": " + currentMod;
				}
			}
			else if (num == (int)NPCBrowserCategories.FilteredNPCs)
			{
				swapFilter = !swapFilter;
				if (swapFilter)
				{
					npcView.selectedCategory = categories[0].Where(x => npcView.allNPCSlot[x].isFiltered).ToArray();
				}
				else
				{
					npcView.selectedCategory = categories[0].Where(x => !npcView.allNPCSlot[x].isFiltered).ToArray();
				}

				npcView.activeSlots = npcView.selectedCategory;
				npcView.ReorderSlots();
				bCategories[num].Tooltip = categNames[num] + " [" + (swapFilter ? "DISABLED" : "ENABLED") + " NPCs]";
			}
			else
			{
				npcView.selectedCategory = categories[num].ToArray();
				npcView.activeSlots = npcView.selectedCategory;
				npcView.ReorderSlots();
			}

			textbox.Text = "";
			UIImage[] array = bCategories;
			for (int j = 0; j < array.Length; j++)
			{
				UIImage uIImage2 = array[j];
				uIImage2.ForegroundColor = buttonColor;
			}

			uIImage.ForegroundColor = buttonSelectedColor;
		}

		private void textbox_KeyPressed(object sender, char key)
		{
			if (textbox.Text.Length <= 0)
			{
				npcView.activeSlots = npcView.selectedCategory;
				npcView.ReorderSlots();
				return;
			}

			List<int> list = new List<int>();
			int[] category = npcView.selectedCategory;
			for (int i = 0; i < category.Length; i++)
			{
				int num = category[i];
				NPCSlot slot = npcView.allNPCSlot[num];
				if (slot.displayName.ToLower().IndexOf(textbox.Text.ToLower(), StringComparison.Ordinal) != -1)
				{
					list.Add(num);
				}
			}

			if (list.Count > 0)
			{
				npcView.activeSlots = list.ToArray();
				npcView.ReorderSlots();
				return;
			}

			textbox.Text = textbox.Text.Substring(0, textbox.Text.Length - 1);
		}

		private void ParseList2()
		{
			//	NPCBrowser.categoryNames = NPCBrowser.categNames.ToList<string>();
			for (int i = 0; i < categNames.Length; i++)
			{
				categories.Add(new List<int>());
				for (int j = 0; j < npcView.allNPCSlot.Length; j++)
				{
					if (i == 0)
					{
						categories[i].Add(j);
						if (npcView.allNPCSlot[j].npc.type >= NPCID.Count)
						{
							string modName = NPCLoader.GetNPC(j).mod.Name;
							List<int> npcInMod;
							if (!ModToNPCs.TryGetValue(modName, out npcInMod)) ModToNPCs.Add(modName, npcInMod = new List<int>());
							npcInMod.Add(j);
						}
					}
					else if (i == 1 && npcView.allNPCSlot[j].npc.boss)
					{
						categories[i].Add(j);
					}
					else if (i == 2 && npcView.allNPCSlot[j].npc.townNPC)
					{
						categories[i].Add(j);
					}
					else if (i == 3 && npcView.allNPCSlot[j].npc.netID < 0)
					{
						categories[i].Add(j);
					}
				}
			}

			npcView.selectedCategory = categories[0].ToArray();
		}

		// Server and Client capable.
		internal static void FilterNPC(int netID, bool desired)
		{
			if (desired)
			{
				if (!filteredNPCSlots.Contains(netID))
				{
					filteredNPCSlots.Add(netID);
				}
			}
			else
			{
				if (filteredNPCSlots.Contains(netID))
				{
					filteredNPCSlots.Remove(netID);
				}
			}
		}
	}
}