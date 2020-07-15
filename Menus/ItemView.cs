using CheatSheet.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.GameContent;
using Terraria.UI;

namespace CheatSheet.Menus
{
	internal class ItemView : UIScrollView
	{
		private float spacing = 8f;

		public Slot[] allItemsSlots;

		private int[] _selectedCategory;

		public int[] activeSlots;

		private int slotSpace = 4;

		private int slotColumns = 10;

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
					Slot slot = allItemsSlots[list[i]];
					if (slot.item.type == 0 || GetTooltipsAsString(slot.item.ToolTip) == "You shouldn't have this")
					{
						list.RemoveAt(i);
						i--;
					}
				}

				_selectedCategory = list.ToArray();
			}
		}

		private string GetTooltipsAsString(ItemTooltip toolTip)
		{
			StringBuilder sb = new StringBuilder();
			for (int j = 0; j < toolTip.Lines; j++)
			{
				sb.Append(toolTip.GetLine(j) + "\n");
			}

			return sb.ToString().ToLower();
		}

		public ItemView()
		{
			Width = (slotSize + slotSpace) * slotColumns + slotSpace + 20f;
			Height = 300f;
			allItemsSlots = new Slot[TextureAssets.Item.Length];
			for (int i = 0; i < allItemsSlots.Length; i++)
			{
				allItemsSlots[i] = new Slot(i);
			}

			//	this.allItemsSlots = (from s in this.allItemsSlots
			//						  select s).ToArray<Slot>();
		}

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
				Slot slot = allItemsSlots[activeSlots[num]];
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