using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Metadata;

namespace SeoEmpire.Common
{
    public static class DbDomain
    {
        private const string sessionKey = "NHib.SessionKey";
		private static ISessionFactory sessionFactory;

		public static ISession CurrentSession
		{
			get
			{
				return GetSession(true);
			}
		}

		public static void InitMySql()
		{
			sessionFactory = new Configuration().Configure("hibernate.cfg.xml").BuildSessionFactory();
		}

        public static void InitSqLite()
        {
            sessionFactory = new Configuration().Configure("sqlite.cfg.xml").BuildSessionFactory();
        }

		public static void Close()
		{
			ISession currentSession = GetSession(false);

			if (currentSession != null)
			{
				currentSession.Close();
			}
		}

		private static ISession GetSession(bool getNewIfNotExists)
		{
		    ISession currentSession = CallContext.GetData(sessionKey) as ISession;

			if (currentSession == null && getNewIfNotExists)
			{
				currentSession = sessionFactory.OpenSession();
				CallContext.SetData(sessionKey, currentSession);
			}
			return currentSession;
		}

        public static IClassMetadata GetMetadata(Type type)
        {
            return sessionFactory.GetClassMetadata(type);
        }
    }
}
