using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Entities
{
    public class Item
    {
        public string CompanyId { get; set; }
        public string ItemId { get; set; }
        [DisplayName("Name")]
        public string ItemName { get; set; }
        [DisplayName("Item Per Box")]
        public int CountPerBox { get; set; }
        [DisplayName("Price Per Item")]
        public double Price { get; set; }
    }

    public class ItemStock : Item
    {
        public string StockId { get; set; }
        [DisplayName("Stock Available")]
        public int CurrentStockTotal { get; set; }
        [DisplayName("Damaged Stock Available")]
        public int CurrentDamagedStockTotal { get; set; }
        public int TotalQuantityToAddToTheCurrentStock { get; set; }
        [DisplayName("Chalan Number")]
        public string ChalanNo { get; set; }
        [DisplayName("Stock Entry Date")]
        public DateTime StockEntryDate { get; set; }

        /// <summary>
        /// This two property depends on CurrentStockTotal and CountPerBox
        /// </summary>
        [DisplayName("Remaining Box Count")]
        public int StockBoxCount { get; set; }
        [DisplayName("Remaining Extra Quantity Count")]
        public int StockNotInBoxCount { get; set; }

        [DisplayName("Damaged Box Count")]
        public int DamagedStockBoxCount { get; set; }
        [DisplayName("Damaged Extra Quantity Count")]
        public int DamagedStockNotInBoxCount { get; set; }

        public ItemStock()
        {
            StockBoxCount = 0;
            StockNotInBoxCount = 0;

            DamagedStockBoxCount = 0;
            DamagedStockNotInBoxCount = 0;
        }

        public void CalculateBoxesFromTotalStock()
        {
            if (CurrentStockTotal != 0 && CountPerBox != 0)
            {
                StockBoxCount = CurrentStockTotal / CountPerBox;
                StockNotInBoxCount = CurrentStockTotal % CountPerBox;
            }
        }

        public void CalculateBoxesFromDamagedTotalStock()
        {
            if (CurrentDamagedStockTotal != 0 && CountPerBox != 0)
            {
                DamagedStockBoxCount = CurrentDamagedStockTotal / CountPerBox;
                DamagedStockNotInBoxCount = CurrentDamagedStockTotal % CountPerBox;
            }
        }
    }

    public class ItemOrder : ItemStock
    {
        public string OrderId { get; set; }
        public string SrId { get; set; }
        public string MarketId { get; set; }
        public DateTime OrderDate { get; set; }

        [DisplayName("Order Count")]
        public int OrderCount { get; set; }
        [DisplayName("Order Box Count")]
        public int OrderBoxCount { get; set; }
        [DisplayName("Order Extra Count")]
        public int OrderExtraCount { get; set; }

        [DisplayName("Returned Count")]
        public int ReturnCount { get; set; }
        [DisplayName("Returned Box Count")]
        public int ReturnBoxCount { get; set; }
        [DisplayName("Returned Extra Count")]
        public int ReturnExtraCount { get; set; }

        [DisplayName("Damaged Count")]
        public int DamageCount { get; set; }
        [DisplayName("Damaged Box Count")]
        public int DamageBoxCount { get; set; }
        [DisplayName("Damaged Extra Count")]
        public int DamageExtraCount { get; set; }

        [DisplayName("Sold Count")]
        public int SellsCount { get; set; }
        [DisplayName("Sold Box Count")]
        public int SellsBoxCount { get; set; }
        [DisplayName("Sold Extra Count")]
        public int SellsExtraCount { get; set; }

        [DisplayName("Taka")]
        public double TotalPrice { get; set; }

        public bool IsItemReturned { get; set; }

        public bool IsDamagedItemReturned { get; set; }

        public ItemOrder() {

            TotalPrice = 0;
            IsItemReturned = false;
            IsDamagedItemReturned = false;
        }

        public void CalculateOrdersInBox()
        {

            if (OrderCount != 0 && CountPerBox != 0)
            {
                OrderBoxCount = OrderCount / CountPerBox;
                OrderExtraCount = OrderCount % CountPerBox;
            }

            if (ReturnCount != 0 && CountPerBox != 0)
            {
                ReturnBoxCount = ReturnCount / CountPerBox;
                ReturnExtraCount = ReturnCount % CountPerBox;
            }

            if (DamageCount != 0 && CountPerBox != 0)
            {
                DamageBoxCount = DamageCount / CountPerBox;
                DamageExtraCount = DamageCount % CountPerBox;
            }

            if (SellsCount != 0 && CountPerBox != 0)
            {
                SellsBoxCount = SellsCount / CountPerBox;
                SellsExtraCount = SellsCount % CountPerBox;
            }
        }
    }
}
