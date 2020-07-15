using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;

namespace CheatSheet.Menus
{
	internal class NPCView : UIScrollView
	{
		private float spacing = 8f;

		public NPCSlot[] allNPCSlot;

		public NPCSlot[] negativeNPCSlots;

		private int[] _selectedCategory;

		public int[] activeSlots;

		private int slotSpace = 4;

		private int slotColumns = 8;

		public int negativeSlots = 65; // number of netIDs < 0

		private float slotSize = Slot.backgroundTexture.Width * 0.85f;

		private int slotRows = 6;

		public int[] selectedCategory
		{
			get { return _selectedCategory; }
			set
			{
				List<int> list = value.ToList();

				for (int i = 0; i < list.Count; i++)
				{
					NPCSlot slot = allNPCSlot[list[i]];
					if (slot.npcType == 0)
					{
						list.RemoveAt(i);
						i--;
					}
				}

				_selectedCategory = list.ToArray();
			}
		}

		public NPCView()
		{
			Width = (slotSize + slotSpace) * slotColumns + slotSpace + 20f;
			Height = 200f;
			allNPCSlot = new NPCSlot[TextureAssets.Npc.Length + negativeSlots];
			for (int i = 0; i < allNPCSlot.Length; i++)
			{
				int type = i >= allNPCSlot.Length - negativeSlots ? -(i - allNPCSlot.Length + negativeSlots + 1) : i;
				allNPCSlot[i] = new NPCSlot(type, i);
			}

			//	this.allNPCSlot = (from s in this.allNPCSlot
			//					   select s).ToArray<NPCSlot>();
		}

		/*	this.allNPCSlot = new NPCSlot[TextureAssets.Npc.Length + 65];
			int index = 0;
			for (int i = -65; i < TextureAssets.Npc.Length; i++)
			{
				//if (i == 0) continue;
				this.allNPCSlot[index] = new NPCSlot(i);
				index++;
			}*/

		public override void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
		}

		public void ReorderSlots()
		{
			ScrollPosition = 0f;
			ClearContent();
			for (int i = 0; i < activeSlots.Length; i++)
			{
				int num = i;
				NPCSlot slot = allNPCSlot[activeSlots[num]];
				int num2 = i % slotColumns;
				int num3 = i / slotColumns;
				float x = slotSpace + num2 * (slot.Width + slotSpace);
				float y = slotSpace + num3 * (slot.Height + slotSpace);
				slot.Position = new Vector2(x, y);
				slot.Offset = Vector2.Zero;
				AddChild(slot);
			}

			ContentHeight = GetLastChild().Y + GetLastChild().Height + spacing;
		}
	}
}