using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client.Listeners;
using Raven.Json.Linq;

namespace FPDinner.Models.Migrations
{
	//public class OrderV1ToV2 : IDocumentConversionListener
	//{
	//	public void DocumentToEntity(string key, object entity, RavenJObject document, RavenJObject metadata)
	//	{
	//		Order o = entity as Order;
	//		if (o != null)
	//		{
	//			if( metadata.Value<int>("Schema-Version") >= 2)
	//			{
	//				return;
	//			}
				
	//			o.Salads = new int[2];
	//		}

	//		OrderedDinner d = entity as OrderedDinner;
	//		if (d != null)
	//		{
	//			if (metadata.Value<int>("Schema-Version") >= 2)
	//			{
	//				return;
	//			}

	//			d.Dinner = 0;
	//		}
	//	}

	//	public void EntityToDocument(string key, object entity, RavenJObject document, RavenJObject metadata)
	//	{
	//		Order o = entity as Order;
	//		OrderedDinner d = entity as OrderedDinner;
	//		if (o != null || d != null)
	//		{
	//			metadata["Schema-Version"] = 2;
	//		}
	//	}
	//}
}