using Mvc.JQuery.Datatables;
using Mvc.JQuery.Datatables.Models;
using Mvc.JQuery.Datatables.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeSatoshi.Common.DataTables;

namespace TradeSatoshi.Core.Helpers
{
	public static class DataTableHelpers
	{
		public static DataTablesParam ToDataTablesParam(this DataTablesModel model)
		{
			return new DataTablesParam
			{
				iDisplayStart = model.iDisplayStart,
				iDisplayLength = model.iDisplayLength,
				iColumns = model.iColumns,
				sSearch = model.sSearch,
				bEscapeRegex = model.bEscapeRegex,
				iSortingCols = model.iSortingCols,
				sEcho = model.sEcho,
				sColumnNames = model.sColumnNames,
				bSortable = model.bSortable,
				bSearchable = model.bSearchable,
				sSearchValues = model.sSearchValues,
				iSortCol = model.iSortCol,
				sSortDir = model.sSortDir,
				bEscapeRegexColumns = model.bEscapeRegexColumns
			};
		}

		public static DataTablesResponse ToDataTablesResponse(this DataTablesResponseData data)
		{
			return new DataTablesResponse
			{
				sEcho = data.sEcho,
				aaData = data.aaData,
				iTotalDisplayRecords = data.iTotalDisplayRecords,
				iTotalRecords = data.iTotalRecords
			};
		}

		public static DataTablesResponse GetDataTableResult<T>(this IQueryable<T> query, DataTablesModel param)
		{
			var dataTablesResponseData = param.ToDataTablesParam().GetDataTablesResponse(query);
			dataTablesResponseData.iTotalDisplayRecords = dataTablesResponseData.iTotalRecords;
			var responseOptions = new ResponseOptions<T>() { ArrayOutputType = null };
			var dictionaryTransform = DataTablesTypeInfo<T>.ToDictionary(responseOptions);
			dataTablesResponseData = dataTablesResponseData.Transform<T, Dictionary<string, object>>(dictionaryTransform)
					   .Transform<Dictionary<string, object>, Dictionary<string, object>>(StringTransformers.StringifyValues);
			dataTablesResponseData = ApplyOutputRules(dataTablesResponseData, responseOptions);
			return dataTablesResponseData.ToDataTablesResponse();
		}

		public static DataTablesResponse GetEmptyDataTableResult(DataTablesModel param)
		{
			return new DataTablesResponse
			{
				sEcho = param.sEcho,
				aaData = new object[]{},
				iTotalDisplayRecords = 0,
				iTotalRecords = 0
			};
		}

		private static DataTablesResponseData ApplyOutputRules<TSource>(DataTablesResponseData sourceData, ResponseOptions<TSource> responseOptions)
		{
			responseOptions = responseOptions ?? new ResponseOptions<TSource>() { ArrayOutputType = ArrayOutputType.BiDimensionalArray };
			DataTablesResponseData outputData = sourceData;

			switch (responseOptions.ArrayOutputType)
			{
				case ArrayOutputType.ArrayOfObjects:
					// Nothing is needed
					break;
				case ArrayOutputType.BiDimensionalArray:
				default:
					outputData = sourceData.Transform<Dictionary<string, object>, object[]>(d => d.Values.ToArray());
					break;
			}

			return outputData;
		}
	}
}
