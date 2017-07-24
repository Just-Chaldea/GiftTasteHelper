﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using StardewValley;

namespace GiftTasteHelper.Framework
{
    internal class NpcGiftInfo
    {
        /*********
        ** Accessors
        *********/
        public SVector2 MaxGiftNameSize { get; private set; }
        public string Name { get; }
        public ItemData[] FavouriteGifts { get; }


        /*********
        ** Public methods
        *********/
        public NpcGiftInfo(string name, int[] favouriteGiftIDs, int maxGiftsToDisplay)
        {
            this.Name = name;
            this.MaxGiftNameSize = SVector2.Zero;

            int numGiftsToDisplay = this.CalculateNumberOfGiftsToDisplay(favouriteGiftIDs.Length, maxGiftsToDisplay);

            this.FavouriteGifts = this.ParseGifts(favouriteGiftIDs, numGiftsToDisplay);
        }


        /*********
        ** Private methods
        *********/
        private ItemData[] ParseGifts(int[] ids, int numToDisplay)
        {
            Debug.Assert(numToDisplay <= ids.Length);

            var itemList = new List<ItemData>(numToDisplay);
            for (int i = 0; i < numToDisplay; ++i)
            {
                if (!Game1.objectInformation.ContainsKey(ids[i]))
                {
                    Utils.DebugLog("Could not find item information for ID: " + ids[i]);
                    continue;
                }

                string objectInfo = Game1.objectInformation[ids[i]];
                string[] parts = objectInfo.Split('/');

                var itemData = new ItemData
                {
                    Name = parts[ItemData.NameIndex],
                    DisplayName = parts[ItemData.DisplayNameIndex],
                    ID = ids[i],
                    TileSheetSourceRect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, ids[i], 16, 16)
                };
                itemList.Add(itemData);

                SVector2 nameLength = SVector2.MeasureString(itemData.DisplayName, Game1.smallFont);
                if (nameLength.XInt > this.MaxGiftNameSize.XInt)
                    this.MaxGiftNameSize = nameLength;
            }
            return itemList.ToArray();
        }

        private int CalculateNumberOfGiftsToDisplay(int numGifts, int maxGiftsToDisplay)
        {
            // 0 or less means no limit
            if (maxGiftsToDisplay <= 0)
                return numGifts;
            return Math.Min(numGifts, maxGiftsToDisplay);
        }
    }
}