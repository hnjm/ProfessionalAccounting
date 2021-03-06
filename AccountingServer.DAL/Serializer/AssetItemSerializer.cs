using AccountingServer.Entities;
using MongoDB.Bson.IO;

namespace AccountingServer.DAL.Serializer
{
    /// <summary>
    ///     折旧计算表条目序列化器
    /// </summary>
    internal class AssetItemSerializer : BaseSerializer<AssetItem>
    {
        public override AssetItem Deserialize(IBsonReader bsonReader)
        {
            string read = null;

            bsonReader.ReadStartDocument();
            var vid = bsonReader.ReadObjectId("voucher", ref read);
            var dt = bsonReader.ReadDateTime("date", ref read);
            var rmk = bsonReader.ReadString("remark", ref read);

            AssetItem item;
            double? val;
            if ((val = bsonReader.ReadDouble("acq", ref read)).HasValue)
                item = new AcquisationItem { VoucherID = vid, Date = dt, Remark = rmk, OrigValue = val.Value };
            else if ((val = bsonReader.ReadDouble("dep", ref read)).HasValue)
                item = new DepreciateItem { VoucherID = vid, Date = dt, Remark = rmk, Amount = val.Value };
            else if ((val = bsonReader.ReadDouble("devto", ref read)).HasValue)
                item = new DevalueItem { VoucherID = vid, Date = dt, Remark = rmk, FairValue = val.Value };
            else if (bsonReader.ReadNull("dispo", ref read))
                item = new DispositionItem { VoucherID = vid, Date = dt, Remark = rmk };
            else
                item = null;
            bsonReader.ReadEndDocument();
            return item;
        }

        public override void Serialize(IBsonWriter bsonWriter, AssetItem item)
        {
            bsonWriter.WriteStartDocument();
            bsonWriter.WriteObjectId("voucher", item.VoucherID);
            bsonWriter.Write("date", item.Date);
            bsonWriter.Write("remark", item.Remark);

            if (item is AcquisationItem acq)
                bsonWriter.Write("acq", acq.OrigValue);
            else if (item is DepreciateItem dep)
                bsonWriter.Write("dep", dep.Amount);
            else if (item is DevalueItem dev)
                bsonWriter.Write("devto", dev.FairValue);
            else if (item is DispositionItem)
                bsonWriter.WriteNull("dispo");
            bsonWriter.WriteEndDocument();
        }
    }
}
