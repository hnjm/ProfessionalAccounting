﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountingServer.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace AccountingServer.DAL
{
    internal class ObjectIdWrapper : IObjectID
    {
        internal ObjectId ID;
    }

    internal static class BsonHelper
    {
        public static IObjectID Wrap(this ObjectId id) { return new ObjectIdWrapper { ID = id }; }

        public static ObjectId UnWrap(this IObjectID id)
        {
            var idWrapper = id as ObjectIdWrapper;
            if (idWrapper == null)
                throw new InvalidOperationException();
            return idWrapper.ID;
        }

        public static BsonDocument ToBsonDocument(this Voucher voucher)
        {
            var doc = new BsonDocument { { "date", voucher.Date } };
            if (voucher.Type != VoucherType.Ordinal)
                switch (voucher.Type)
                {
                    case VoucherType.Amortization:
                        doc.Add("special", "amorz");
                        break;
                    case VoucherType.AnnualCarry:
                        doc.Add("special", "acarry");
                        break;
                    case VoucherType.Carry:
                        doc.Add("special", "carry");
                        break;
                    case VoucherType.Depreciation:
                        doc.Add("special", "dep");
                        break;
                    case VoucherType.Devalue:
                        doc.Add("special", "dev");
                        break;
                    case VoucherType.Uncertain:
                        doc.Add("special", "unc");
                        break;
                }
            if (voucher.Details != null)
                foreach (var detail in voucher.Details)
                    doc.Add(detail.ToBsonDocument());
            if (voucher.Remark != null)
                doc.Add("remark", voucher.Remark);

            return doc;
        }

        public static BsonDocument ToBsonDocument(this VoucherDetail detail)
        {
            var val = new BsonDocument { { "title", detail.Title } };
            if (detail.SubTitle.HasValue)
                val.Add("subtitle", detail.SubTitle);
            if (detail.Content != null)
                val.Add("content", detail.Content);
            val.Add("fund", detail.Fund);
            if (detail.Remark != null)
                val.Add("content", detail.Remark);
            return val;
        }

        public static Voucher ToVoucher(this BsonDocument doc)
        {
            var voucher = new Voucher { ID = doc["_id"].AsObjectId.Wrap() };
            if (!doc["date"].IsBsonNull)
                voucher.Date = doc["date"].AsDateTime;
            voucher.Type = VoucherType.Ordinal;
            if (!doc["special"].IsBsonNull)
                switch (doc["special"].AsString)
                {
                    case "amorz":
                        voucher.Type = VoucherType.Amortization;
                        break;
                    case "acarry":
                        voucher.Type = VoucherType.AnnualCarry;
                        break;
                    case "carry":
                        voucher.Type = VoucherType.Carry;
                        break;
                    case "dep":
                        voucher.Type = VoucherType.Depreciation;
                        break;
                    case "dev":
                        voucher.Type = VoucherType.Devalue;
                        break;
                    case "unc":
                        voucher.Type = VoucherType.Uncertain;
                        break;
                }
            if (!doc["detail"].IsBsonNull)
            {
                var ddocs = doc["detail"].AsBsonArray;
                var details = new VoucherDetail[ddocs.Count];
                for (var i = 0; i < ddocs.Count; i++)
                    details[i] = ddocs[i].AsBsonDocument.ToVoucherDetail();
                
            }
            if (!doc["remark"].IsBsonNull)
                voucher.Remark = doc["remark"].AsString;

            return voucher;
        }

        public static VoucherDetail ToVoucherDetail(this BsonDocument ddoc)
        {
            var detail = new VoucherDetail();
            if (!ddoc["title"].IsBsonNull)
                detail.Title = ddoc["title"].AsInt32;
            if (!ddoc["subtitle"].IsBsonNull)
                detail.SubTitle = ddoc["subtitle"].AsInt32;
            if (!ddoc["content"].IsBsonNull)
                detail.Content = ddoc["content"].AsString;
            if (!ddoc["fund"].IsBsonNull)
                detail.Fund = ddoc["fund"].AsDouble;
            if (!ddoc["remark"].IsBsonNull)
                detail.Remark = ddoc["remark"].AsString;
            return detail;
        }
    }
    
    public class MongoDbHelper : IDbHelper
    {
        private MongoClient m_Client;
        private MongoServer m_Server;
        private MongoDatabase m_Db;

        private MongoCollection m_Vouchers;

        public MongoDbHelper()
        {
            m_Client = new MongoClient("mongodb://localhost");
            m_Server = m_Client.GetServer();
            m_Db = m_Server.GetDatabase("accounting");

            m_Vouchers = m_Db.GetCollection("voucher");

            m_Server.Connect();
        }

        public void Dispose()
        {
            if (m_Server != null)
            {
                m_Server.Disconnect();
                m_Server = null;
            }
        }

        public Voucher SelectVoucher(IObjectID id)
        {
            return m_Vouchers.FindOneByIdAs<BsonDocument>(id.UnWrap()).ToVoucher();
        }

        public IEnumerable<Voucher> SelectVouchers(Voucher filter)
        {
            var lst = new List<IMongoQuery>();

            return m_Vouchers.FindAs<BsonDocument>(Query.And(lst)).Select(d => d.ToVoucher());
        }
        public int SelectVouchersCount(Voucher filter) { throw new NotImplementedException(); }
        public bool InsertVoucher(Voucher entity) { throw new NotImplementedException(); }
        public int DeleteVouchers(Voucher filter) { throw new NotImplementedException(); }
        public VoucherDetail SelectDetail(IObjectID id) { throw new NotImplementedException(); }
        public IEnumerable<Voucher> SelectItemsWithDetail(VoucherDetail filter) { throw new NotImplementedException(); }
        public IEnumerable<VoucherDetail> SelectDetails(VoucherDetail filter) { throw new NotImplementedException(); }
        public int SelectDetailsCount(VoucherDetail filter) { throw new NotImplementedException(); }
        public bool InsertDetail(VoucherDetail entity) { throw new NotImplementedException(); }
        public int DeleteDetails(VoucherDetail filter) { throw new NotImplementedException(); }
        public DbAsset SelectAsset(Guid id) { throw new NotImplementedException(); }
        public IEnumerable<DbAsset> SelectAssets(DbAsset filter) { throw new NotImplementedException(); }
        public bool InsertAsset(DbAsset entity) { throw new NotImplementedException(); }
        public int DeleteAssets(DbAsset filter) { throw new NotImplementedException(); }
        public IEnumerable<VoucherDetail> GetXBalances(VoucherDetail filter, bool noCarry = false, int? sID = null, int? eID = null, int dir = 0) { throw new NotImplementedException(); }
        public void Depreciate() { throw new NotImplementedException(); }
        public void Carry() { throw new NotImplementedException(); }
        public IEnumerable<DailyBalance> GetDailyBalance(decimal title, string remark, int dir = 0) { throw new NotImplementedException(); }
        public IEnumerable<DailyBalance> GetDailyXBalance(decimal title, int dir = 0) { throw new NotImplementedException(); }
        public string GetFixedAssetName(Guid id) { throw new NotImplementedException(); }
    }
}