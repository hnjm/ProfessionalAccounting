﻿using System;
using System.Collections.Generic;
using AccountingServer.BLL;
using AccountingServer.Entities;

namespace AccountingServer.Shell.Parsing
{
    public partial class ShellParser
    {
        public partial class NamedQueryContext : INamedQuery
        {
            /// <inheritdoc />
            public INamedQuery InnerQuery
            {
                get
                {
                    if (namedQ() != null)
                        return namedQ();
                    if (namedQueries() != null)
                        return namedQueries();
                    if (namedQueryTemplateR() != null)
                        return namedQueryTemplateR();
                    throw new MemberAccessException("表达式错误");
                }
            }

            /// <inheritdoc />
            public string Name => InnerQuery.Name;

            public bool InheritQuery => InnerQuery.InheritQuery;
        }

        public partial class NamedQueryTemplateRContext : INamedQueryTemplateR
        {
            /// <inheritdoc />
            public string Name => name().DollarQuotedString().Dequotation();

            public bool InheritQuery => Inh == null;
        }

        public partial class NamedQueriesContext : INamedQueries
        {
            /// <inheritdoc />
            public string Name => name().DollarQuotedString().Dequotation();

            public bool InheritQuery
            {
                get
                {
                    if (Inh == null)
                        return true;
                    return Inh.Text == "/";
                }
            }

            /// <inheritdoc />
            public double Coefficient
            {
                get
                {
                    if (coef() == null)
                        return 1;
                    if (coef().Percent() != null)
                    {
                        var s = coef().Percent().GetText();
                        return Double.Parse(s.Substring(1, s.Length - 2)) / 100D;
                    }
                    if (coef().Float() != null)
                    {
                        var s = coef().Float().GetText();
                        return Double.Parse(s.Substring(1, s.Length - 1));
                    }
                    throw new MemberAccessException("表达式错误");
                }
            }

            /// <inheritdoc />
            public string Remark => DoubleQuotedString().Dequotation();

            /// <inheritdoc />
            public IReadOnlyList<INamedQuery> Items => namedQuery();

            public IQueryCompunded<IVoucherQueryAtom> CommonQuery => vouchers();
        }

        public partial class NamedQContext : INamedQ
        {
            /// <inheritdoc />
            public string Name => name().DollarQuotedString().Dequotation();

            public bool InheritQuery => Inh == null;

            /// <inheritdoc />
            public double Coefficient
            {
                get
                {
                    if (coef() == null)
                        return 1D;
                    if (coef().Percent() != null)
                    {
                        var s = coef().Percent().GetText();
                        return Double.Parse(s.Substring(1, s.Length - 2)) / 100D;
                    }
                    if (coef().Float() != null)
                    {
                        var s = coef().Float().GetText();
                        return Double.Parse(s.Substring(1, s.Length - 1));
                    }
                    throw new MemberAccessException("表达式错误");
                }
            }

            /// <inheritdoc />
            public string Remark => DoubleQuotedString().Dequotation();

            /// <inheritdoc />
            public IGroupedQuery GroupingQuery => groupedQuery();
        }
    }
}
