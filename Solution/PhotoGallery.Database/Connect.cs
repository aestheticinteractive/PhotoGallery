﻿using System.Configuration;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using PhotoGallery.Database.Maps;

namespace PhotoGallery.Database {

	/*================================================================================================*/
	public static class Connect {

		public static ISessionFactory SessionFactory { get; private set; }

		private static NHibernate.Cfg.Configuration Config;


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void InitOnce() {
			if ( SessionFactory != null ) {
				return;
			}

			IPersistenceConfigurer conn = MsSqlConfiguration
				.MsSql2008
				.DefaultSchema("kpg")
				.Dialect<NHibernate.Dialect.MsSql2008Dialect>()
				.Provider<NHibernate.Connection.DriverConnectionProvider>()
				.Driver<NHibernate.Driver.SqlClientDriver>()
				.AdoNetBatchSize(500)
				.ConnectionString(ConfigurationManager.AppSettings["DbConnStr"]);

			SessionFactory = Fluently.Configure()
				.Database(conn)
				.Mappings(m => m
					.FluentMappings
						.AddFromAssemblyOf<AlbumMap>()
					.Conventions.Add(
						PrimaryKey.Name.Is(x => "Id"),
						ForeignKey.EndsWith("Id"),
						ConventionBuilder.Property.Always(i => i.Not.Nullable()),
						ConventionBuilder.Reference.Always(i => i.Not.Nullable())
					)
				)
				.ExposeConfiguration(c => { Config = c; })
				.BuildSessionFactory();
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public static void UpdateSchema() {
			var schema = new SchemaUpdate(Config);
			schema.Execute(false, true);
		}

		/*--------------------------------------------------------------------------------------------*/
		public static void RebuildDatabase() {
			var schema = new SchemaExport(Config);
			schema.Create(false, true);
		}

	}

}