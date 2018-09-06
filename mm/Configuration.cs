// ----------------------------------------------------------------------
// <copyright file="Configuration.cs" company="Gazprom Space Systems">
// Copyright statement. All right reserved 
// Developer:   Ivan Starski
// Date: 09/06/2017 11:19
// </copyright>
// ----------------------------------------------------------------------
namespace mm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    public static class Configuration
    {
        public static SynchronizationContext Context { get; set; } = SynchronizationContext.Current;
        public static IList<Predicate<IMessage>> Criteria { get; set; } = new List<Predicate<IMessage>>();
        internal static bool CheckCriteries(IMessage message) => Criteria.Count > 0 && Criteria.Any(pr => pr(message));
        public static Action<IMessage> OnLog { get; set; }
    }
}