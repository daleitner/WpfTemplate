using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Input;
using Moq;
using SC.Base.WPF.Commands;
using Match = System.Text.RegularExpressions.Match;

namespace SC.Test.Common.Extensions
{
	public static class PrettyPrintExtension
	{
		#region Separators / Characters

		private static string Bullet1 { get; } = "Δ";
		private static string Bullet2 { get; } = "└➝";

		private static string ObjectSeparator => Environment.NewLine + Environment.NewLine;

		#endregion

		#region Extension Method

		public static string ToPrettyString(this object anything, params object[] furtherObjects)
		{
			string prettyString = string.Empty;

			prettyString += PrettyPrintObject(anything);

			foreach (object anotherObject in furtherObjects)
			{
				prettyString += ObjectSeparator;
				prettyString += PrettyPrintObject(anotherObject);
			}

			return prettyString;
		}

		public static string ToPrettyString(this object anything, List<Type> ignoredTypes)
		{
			_ignoredTypes = ignoredTypes;
			var str = PrettyPrintObject(anything);
			_ignoredTypes = new List<Type>();
			return str;
		}

		

		#endregion

		#region Pretty Methods

		private static readonly List<KeyValuePair<object, int>> PrintedObjects = new List<KeyValuePair<object, int>>();
		private static List<Type> _ignoredTypes = new List<Type>();

		private static string PrettyPrintObject(object anything, string title = "", int inherit = 0)
		{
			string result = Prettify(anything, title, inherit);
			PrintedObjects.Clear();
			return result;
		}

		private static string Prettify(object anything, string title = "", int inherit = 0)
		{
			if (anything == null || ObjectType_Is_NotPrintedType(anything))
				return string.Empty;

			var enumerable = anything as IEnumerable;
			if (enumerable != null)
				return HandleEnumerable(title, inherit, enumerable);

			if (IsAlreadyPrinted(anything))
			{
				var alreadyPrinted = HandleAlreadyPrinted(anything, inherit);
				return alreadyPrinted;
			}
			PrintedObjects.Add(new KeyValuePair<object, int>(anything, inherit));

			return PrettyObject(anything, title, inherit);
		}

		private static string HandleEnumerable(string title, int inherit, IEnumerable enumerable)
		{
			List<object> objectList;
			try
			{
				objectList = enumerable.Cast<object>().ToList();


			}
			catch (Exception ex)
			{
				return HandlePropertyError(ex);
			}

			object first = objectList.FirstOrDefault();
			if (first == null)
				return string.Empty;

			if (string.IsNullOrEmpty(title))
				title = first.GetType().Name + "s";

			if (first.IsSimpleType() || !HasComplexProperties(first))
				return PrettySimpleList(objectList, title, inherit);

			if (first.GetType().Name.ToLower().Contains("keyvaluepair"))
			{
				if (first is KeyValuePair<string, string>)
					return PrettySimpleList(objectList, title, inherit);
			}
			return PrettyComplexList(objectList, title, inherit);
		}

		private static string HandleAlreadyPrinted(object anything, int inherit)
		{
			string tabs = String.Empty;
			for (int i = 0; i < inherit; i++)
				tabs += "\t";

			Dictionary<string, object> simpleProperties = GetProperties(anything, true, false);
			List<string> alreadyPrintedInfos = new List<string>();

			if (simpleProperties.Keys.Any(k => k.ToUpper() == "ID"))
			{
				object objId = simpleProperties.First(kvp => kvp.Key.ToUpper() == "ID").Value;
				if (!String.IsNullOrWhiteSpace(objId?.ToString()))
					alreadyPrintedInfos.Add("ID:" + objId);
			}

			if (simpleProperties.Keys.Any(k => k.ToUpper() == "NAME"))
			{
				object objName = simpleProperties.First(kvp => kvp.Key.ToUpper() == "NAME").Value;
				if (!String.IsNullOrWhiteSpace(objName?.ToString()))
					alreadyPrintedInfos.Add("Name:" + objName);
			}

			string alreadyPrinted = tabs + Bullet2 + " " + anything.GetType().Name + " ";
			alreadyPrinted += alreadyPrintedInfos.Count > 0 ? "(" + String.Join(", ", alreadyPrintedInfos) + ") " : "";
			alreadyPrinted += "[already printed]";
			return alreadyPrinted;
		}

		private static bool IsAlreadyPrinted(object anything)
		{
			return PrintedObjects.Any(kvp => kvp.Key == anything);
		}

		private static string PrettyObject(object anything, string title = "", int inherit = 0)
		{
			if (anything == null)
				return String.Empty;

			StringBuilder prettyString = new StringBuilder();

			if (anything.IsSimpleType())
			{
				if (!String.IsNullOrEmpty(title) && !String.IsNullOrEmpty(anything.ToString()))
				{
					AddHeader(prettyString, title, inherit);
					AddString(prettyString, anything.ToString(), inherit + 1);
				}
				else
					AddString(prettyString, anything.ToString(), inherit);

				return GetString(prettyString);
			}

			if (String.IsNullOrEmpty(title))
				title = anything.GetType().Name;

			StringTable table = new StringTable();

			Dictionary<string, object> simpleProperties = GetProperties(anything, true, false);
			Dictionary<string, object> complexProperties = GetProperties(anything, false, true);

			bool hasSimpleProperties = CheckProperties(simpleProperties);
			bool hasComplexProperties = CheckProperties(complexProperties);

			if (hasSimpleProperties || hasComplexProperties)
				AddHeader(prettyString, title, inherit);

			if (hasSimpleProperties)
			{
				table.AddTitleRow(simpleProperties.Keys.ToArray<object>());
				table.AddRow(simpleProperties.Values.ToArray());
				AddString(prettyString, table.GetFormattedString(inherit));
			}

			if (hasComplexProperties)
			{
				foreach (KeyValuePair<string, object> property in complexProperties)
				{
					if (property.Value == null)
						continue;

					string propertyString;
					if (property.Value is string)
					{
						propertyString = property.Key + " - " + property.Value;
					}
					else
					{
						propertyString = Prettify(property.Value, property.Key, inherit + 1);
					}

					AddString(prettyString, propertyString);
				}
			}

			return prettyString.ToString();
		}

		private static string PrettySimpleList(IEnumerable<object> objects, string title = "", int inherit = 0)
		{
			var enumerable = objects as object[] ?? objects.ToArray();
			if (!enumerable.Any())
				return String.Empty;

			List<object> objectList = enumerable.ToList();

			StringBuilder prettyString = new StringBuilder();

			List<object> namesList = new List<object>();

			if (objectList[0].IsSimpleType())
			{
				if (!String.IsNullOrEmpty(title))
					namesList.Add(title);
			}
			else
			{
				if (!String.IsNullOrEmpty(title))
					AddHeader(prettyString, $"{title} <{objectList.Count}>", inherit);

				namesList = GetProperties(objectList[0], true, true).Keys.ToList<object>();
			}

			StringTable table = new StringTable();
			table.AddTitleRow(namesList.ToArray<object>());

			foreach (object element in objectList.Where(element => element != null))
			{
				table.AddRow(element.IsSimpleType() ? new[] { element } : GetProperties(element, true, true).Values.ToArray());
			}

			AddString(prettyString, table.GetFormattedString(inherit));

			return GetString(prettyString);
		}

		private static string PrettyComplexList(IEnumerable<object> objects, string title = "", int inherit = 0)
		{
			var enumerable = objects as object[] ?? objects.ToArray();
			if (!enumerable.Any())
				return String.Empty;

			List<object> objectList = enumerable.ToList();

			StringBuilder prettyString = new StringBuilder();

			if (!String.IsNullOrEmpty(title))
				AddHeader(prettyString, $"{title} <{objectList.Count}>", inherit);

			foreach (object element in objectList)
			{
				if (element == null)
					continue;

				AddString(prettyString, !string.IsNullOrEmpty(title) ? Prettify(element, "", inherit + 1) 
					: Prettify(element, "", inherit));
			}

			return GetString(prettyString);
		}

		#endregion

		#region Helper

		private static bool HasComplexProperties(object obj)
		{
			Dictionary<string, object> properties = GetProperties(obj, false, true);
			if (properties != null && properties.Count > 0)
				return true;

			return false;
		}

		private static Dictionary<string, object> GetProperties(object obj, bool simpleTypes, bool complexTypes)
		{
			Dictionary<string, object> properties = new Dictionary<string, object>();

			if (obj == null)
				return properties;


			var propertyInfoList = GetSortedPropertyInfoList(obj);

			foreach (PropertyInfo property in propertyInfoList)
			{
				var propertyType = property.PropertyType;
				if ((simpleTypes && propertyType.IsSimpleType()) || (complexTypes && !propertyType.IsSimpleType()))
				{
					if (properties.All(item => !string.Equals(item.Key, property.Name, StringComparison.CurrentCultureIgnoreCase)))
					{
						object propertyValue;
						try
						{
							propertyValue = property.GetValue(obj, null);
						}
						catch (Exception ex)
						{
							propertyValue = HandlePropertyError(ex);
						}

						properties.Add(property.Name, propertyValue);
					}
				}
			}

			if (complexTypes)
			{
				var fields = GetComplexFieldsOrderedByName(obj);
				foreach (FieldInfo field in fields)
				{
					
					try
					{
						string fieldNameOriginal = field.Name;
						Match m = Regex.Match(field.Name, "<(.*)>.*");
						if (m.Success && m.Groups.Count == 2)
							fieldNameOriginal = m.Groups[1].Value;

						var fieldNameWithoutUnderscore = fieldNameOriginal.StartsWith("_") ? fieldNameOriginal.Substring(1) : fieldNameOriginal;

						if (properties.All(property => !string.Equals(property.Key, fieldNameWithoutUnderscore, StringComparison.CurrentCultureIgnoreCase)))
						{
							object fieldValue;
							try
							{
								fieldValue = field.GetValue(obj);
							}
							catch (Exception ex)
							{
								fieldValue = HandlePropertyError(ex);
							}

							properties.Add(fieldNameOriginal, fieldValue);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}


			return properties;
		}

		private static string HandlePropertyError(Exception ex)
		{
			var exc = ex;
				while (exc.InnerException != null)
					exc = exc.InnerException;
				var errorMessage = "Error: " + exc.Message;
			return errorMessage;
		}

		private static IList<PropertyInfo> GetSortedPropertyInfoList(object obj)
		{
			var propertyInfos = obj.GetType().GetProperties();

			var propertyInfoList = SortArrayToList(propertyInfos);
			return propertyInfoList;
		}

		private static IEnumerable<FieldInfo> GetComplexFieldsOrderedByName(object obj)
		{
			FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			return fields.Where(IsRelevantComplexField).OrderBy(f => f.Name);
		}

		private static bool IsRelevantComplexField(FieldInfo f)
		{
			return !f.FieldType.IsSimpleType() && f.FieldType.BaseType != typeof (MulticastDelegate) && !f.FieldType.IsInterface;
		}

		private static bool IsSimpleType(this object objectToCheck)
		{
			if (objectToCheck == null)
				return false;

			return objectToCheck.GetType().IsSimpleType();
		}

		private static bool IsSimpleType(this Type typeToCheck)
		{
			var typeCode = Type.GetTypeCode(GetUnderlyingType(typeToCheck));

			switch (typeCode)
			{
				case TypeCode.Boolean:
				case TypeCode.Byte:
				case TypeCode.Char:
				case TypeCode.DateTime:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single:
				case TypeCode.String:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return true;
			}
			if (typeToCheck == typeof (ICommand) || typeToCheck == typeof(RelayCommand))
				return true;
			return false;
		}

		private static Type GetUnderlyingType(Type typeToCheck)
		{
			if (typeToCheck.IsGenericType && typeToCheck.GetGenericTypeDefinition() == typeof(Nullable<>))
				return Nullable.GetUnderlyingType(typeToCheck);

			return typeToCheck;
		}

		private static string GetString(StringBuilder sb)
		{
			return sb.ToString().TrimEnd('\r', '\n');
		}

		private static void AddHeader(StringBuilder sb, string title, int inherit = 0)
		{
			title = FormatTitleString(title);

			string tabs = String.Empty;
			for (int i = 0; i < inherit; i++)
				tabs += "\t";

			string bullet = Bullet1;

			if (inherit > 0)
				bullet = Bullet2;

			if (inherit == 0 && !String.IsNullOrEmpty(sb.ToString()))
				AddEmptyLine(sb);

			AddString(sb, tabs + bullet + " " + title);
		}

		private static string FormatTitleString(string title)
		{
			if (String.IsNullOrWhiteSpace(title))
				return title;

			if (title.Length > 1)
			{
				char[] titleChars = title.ToArray();
				for (int i = 0; i < titleChars.Length; i++)
				{
					if (i == 0)
						titleChars[i] = Char.ToUpper(titleChars[i]);

					if (Char.IsDigit(titleChars[i]) && i <= titleChars.Length - 2)
						titleChars[i + 1] = Char.ToUpper(titleChars[i + 1]);
				}

				return String.Join("", titleChars);
			}

			return title.ToUpper();
		}

		private static void AddEmptyLine(StringBuilder sb)
		{
			sb.AppendLine("");
		}

		private static void AddString(StringBuilder sb, string text, int inherit = 0)
		{
			if (String.IsNullOrEmpty(text))
				return;

			string tabs = String.Empty;

			for (int i = 0; i < inherit; i++)
				tabs += "\t";

			sb.AppendLine(tabs + text.TrimEnd('\r', '\n'));
		}

		private static IList<PropertyInfo> SortArrayToList(PropertyInfo[] propertyInfos, string firstElementName = "ID")
		{
			List<PropertyInfo> existingList = propertyInfos.ToList();
			List<PropertyInfo> newList = new List<PropertyInfo>();

			if (!String.IsNullOrEmpty(firstElementName) && existingList.Any(p => p.Name.ToUpper() == firstElementName.ToUpper()))
			{
				PropertyInfo firstElement = existingList.Single(p => p.Name.ToUpper() == firstElementName.ToUpper());
				if (firstElement != null)
				{
					existingList.Remove(firstElement);
					newList.Add(firstElement);
				}
			}

			newList.AddRange(existingList.OrderBy(p => p.Name));

			return newList;
		}

		private static bool CheckProperties(Dictionary<string, object> properties)
		{
			foreach (KeyValuePair<string, object> property in properties)
			{
				var value = property.Value as string;
				if (value != null)
				{
					if (!string.IsNullOrEmpty(value))
						return true;
				}
				else
					if (property.Value != null)
					return true;
			}

			return false;
		}

		private static bool ObjectType_Is_NotPrintedType(object anything)
		{
			if (anything is Mock ||
				//anything is Microsoft.Practices.Prism.Events.EventAggregator ||
				anything is CultureInfo ||
				anything is Exception ||
				anything is Timer)
				return true;

			return _ignoredTypes.Contains(anything.GetType());

		}

		#endregion
	}
}
