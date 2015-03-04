using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
 
public class JSONObject {
	
	public enum TYPE {
		OBJECT,
		LIST,
		STRING,
		DOUBLE,
		LONG,
		BOOL,
		NULL
	} // TYPE
	
	private TYPE _type;
	private Object _value;
	
	public JSONObject(Object value) {
		if (value == null) {
			throw new ArgumentNullException("null");
		} else if (value is Dictionary<string, JSONObject>) {
			this._type = TYPE.OBJECT;
			this._value = value;
		} else if (value is List<JSONObject>) {
			this._type = TYPE.LIST;
			this._value = value;
		} else if (value is string) {
			this._type = TYPE.STRING;
			this._value = value;
		} else if (value is double) {
			this._type = TYPE.DOUBLE;
			this._value = value;
		} else if (value is float) {
			this._type = TYPE.DOUBLE;
			this._value = (double)value;
		} else if (value is long) {
			this._type = TYPE.LONG;
			this._value = value;
		} else if (value is int) {
			this._type = TYPE.LONG;
			this._value = Convert.ToInt64(value);
		} else if (value is bool) {
			this._type = TYPE.BOOL;
			this._value = (bool)value;
		} else {
			throw new ArgumentException("stupid type");
		}
	} // JSONObject()
	
	/*
	
	public JSONObject(Dictionary<string, JSONObject> value) {
		this._type = TYPE.OBJECT;
		this._value = value;
	}
	
	public JSONObject(List<JSONObject> value) {
		this._type = TYPE.LIST;
		this._value = value;
	}
	
	public JSONObject(string value) {
		this._type = TYPE.STRING;
		this._value = value;
	}
	
	public JSONObject(double value) {
		this._type = TYPE.DOUBLE;
		this._value = value;
	}
	
	public JSONObject(float value) {
		this._type = TYPE.DOUBLE;
		this._value = (double)value;
	}
	
	public JSONObject(long value) {
		this._type = TYPE.LONG;
		this._value = value;
	}
	
	public JSONObject(int value) {
		this._type = TYPE.LONG;
		this._value = (long)value;
	}
	
	public JSONObject(bool value) {
		this._type = TYPE.BOOL;
		this._value = value;
	}
	
	public JSONObject() {
		this._type = TYPE.NULL;
		this._value = null;
	}
	
	*/
	
	public new Type GetType() {
		return this._value.GetType();
	} // GetType()
	
	public int Count {
		get {
			if (this._type == TYPE.OBJECT) {
				return ((Dictionary<string, JSONObject>)(this._value)).Count;
			} else if (this._type == TYPE.LIST) {
				return ((List<JSONObject>)(this._value)).Count;
			} else {
				throw new ArgumentException("stupid type");
			}
		}
	}
	
	public JSONObject this[string index] {
		get {
			if (this._type == TYPE.OBJECT) {
				return ((Dictionary<string, JSONObject>)(this._value))[index];
			} else {
				throw new ArgumentException("stupid type");
				// return null;
			}
		}
		set {
			if (this._type == TYPE.OBJECT && value is JSONObject) {
				((Dictionary<string, JSONObject>)(this._value))[index] = value != null ? (value is JSONObject ? (JSONObject)value : new JSONObject(value)) : null;
			} else {
				throw new ArgumentException("stupid type");
			}
		}
	}
	
	public JSONObject this[int index] {
		get {
			if (this._type == TYPE.LIST) {
				return ((List<JSONObject>)(this._value))[index];
			} else {
				throw new ArgumentException("stupid type");
				// return null;
			}
		}
		set {
			if (this._type == TYPE.LIST) {
				((List<JSONObject>)(this._value))[index] = value != null ? (value is JSONObject ? (JSONObject)value : new JSONObject(value)) : null;
			} else {
				throw new ArgumentException("stupid type");
			}
		}
	}
	
	public static explicit operator Dictionary<string, JSONObject> (JSONObject data) {
		if (data._type != TYPE.OBJECT) {
			throw new InvalidCastException("Instance of JSONObject doesn't hold a Dictionary<string, JSONObject>");
		}
		return (Dictionary<string, JSONObject>)(data._value);
	}
	
	public static explicit operator List<JSONObject> (JSONObject data) {
		if (data._type != TYPE.LIST) {
			throw new InvalidCastException("Instance of JSONObject doesn't hold a List<JSONObject>");
		}
		return (List<JSONObject>)(data._value);
	}
	
	public static explicit operator string (JSONObject data) {
		if (data._type != TYPE.STRING) {
			throw new InvalidCastException("Instance of JSONObject doesn't hold a string");
		}
		return (string)(data._value);
	}
	
	public static explicit operator double (JSONObject data) {
		if (data._type == TYPE.DOUBLE) {
			return (double)(data._value);
		} else if (data._type == TYPE.LONG) {
			return (double)(long)(data._value);
		} else if (data._type == TYPE.STRING) {
			try {
				return double.Parse((string)data._value);
			} catch {
				throw new InvalidCastException("Instance of JSONObject doesn't hold a double or float");
			}
		} else if (data._type == TYPE.BOOL) {
			return (bool)data._value ? 1.0 : 0.0;
		}
		throw new InvalidCastException("Instance of JSONObject doesn't hold a double or float");
	}
	
	public static explicit operator float (JSONObject data) {
		return (float)(double)data;
	}
	
	public static explicit operator long (JSONObject data) {
		if (data._type == TYPE.DOUBLE) {
			return (long)(double)(data._value);
		} else if (data._type == TYPE.LONG) {
			return (long)(data._value);
		} else if (data._type == TYPE.STRING) {
			try {
				return long.Parse((string)data._value);
			} catch {
				throw new InvalidCastException("Instance of JSONObject doesn't hold a long or int");
			}
		} else if (data._type == TYPE.BOOL) {
			return (bool)data._value ? 1 : 0;
		}
		throw new InvalidCastException("Instance of JSONObject doesn't hold a long or int");
	}
	
	public static explicit operator int (JSONObject data) {
		return (int)(long)data;
	}
	
	public static explicit operator bool (JSONObject data) {
		if (data._type != TYPE.BOOL) {
			throw new InvalidCastException("Instance of JSONObject doesn't hold a bool");
		} else if (data._type == TYPE.STRING) {
			try {
				return bool.Parse((string)data._value);
			} catch {
				throw new InvalidCastException("Instance of JSONObject doesn't hold a bool");
			}
		}
		return (bool)data._value;
	}
	
	public override string ToString() {
		if (this._type == TYPE.STRING) {
			return (string)this._value;
		}
		return this._value.ToString();
	}
	
	public Dictionary<string, JSONObject> ToDictionary() {
		return (Dictionary<string, JSONObject>)this;
	}
	
	public List<JSONObject> ToList() {
		return (List<JSONObject>)this;
	}
	
	public double ToDouble() {
		return (double)this;
	}
	
	public float ToFloat() {
		return (float)this;
	}
	
	public long ToLong() {
		return (long)this;
	}
	
	public int ToInt() {
		return (int)this;
	}
	
	public bool ToBool() {
		return (bool)this;
	}
	
	/**/
	
	public void SetObject(Object obj)
	{
		_value = obj;
		
		if(obj is double || obj is float)
		{
			_type = TYPE.DOUBLE;
		}
		else if(obj is long || obj is int)
		{
			_type = TYPE.LONG;
		}
	}
	
	private static Dictionary<JSONObject, JSONObject> _forNext = new Dictionary<JSONObject, JSONObject>();
	
	public static JSONObject CreateObject() {
		return new JSONObject(new Dictionary<string, JSONObject>());
	}
	
	public static JSONObject CreateList() {
		return new JSONObject(new List<JSONObject>());
	}
	
	public JSONObject AddList(string key) {
		if (this._type != TYPE.OBJECT) {
			throw new InvalidCastException("Instance of JSONObject doesn't hold a Dictionary<string, JSONObject>");
		}
		JSONObject result = JSONObject.CreateList();
		((Dictionary<string, JSONObject>)(this._value)).Add(key, result);
		JSONObject._forNext.Add(result, this);
		return result;
	}
	
	public JSONObject AddItem(Object value) {
		if (this._type != TYPE.LIST) {
			throw new InvalidCastException("Instance of JSONObject doesn't hold a List<JSONObject>");
		}
		JSONObject item = value != null ? (value is JSONObject ? (JSONObject)value : new JSONObject(value)) : null;
		((List<JSONObject>)(this._value)).Add(item);
		return this;
	}
	
	public JSONObject Next() {
		JSONObject result = JSONObject._forNext[this];
		JSONObject._forNext.Remove(this);
		return result;
	}
	
	public JSONObject AddItem(string key, Object value) {
		if (this._type != TYPE.OBJECT) {
			throw new InvalidCastException("Instance of JSONObject doesn't hold a Dictionary<string, JSONObject>");
		}
		JSONObject item = value != null ? (value is JSONObject ? (JSONObject)value : new JSONObject(value)) : null;
		((Dictionary<string, JSONObject>)(this._value)).Add(key, item);
		return this;
	}
	
	public JSONObject AddObject(string key) {
		if (this._type != TYPE.OBJECT) {
			throw new InvalidCastException("Instance of JSONObject doesn't hold a Dictionary<string, JSONObject>");
		}
		JSONObject result = JSONObject.CreateObject();
		((Dictionary<string, JSONObject>)(this._value)).Add(key, result);
		JSONObject._forNext.Add(result, this);
		return result;
	}
	
	public JSONObject AddObject() {
		if (this._type != TYPE.LIST) {
			throw new InvalidCastException("Instance of JSONObject doesn't hold a List<JSONObject>");
		}
		JSONObject result = JSONObject.CreateObject();
		((List<JSONObject>)(this._value)).Add(result);
		JSONObject._forNext.Add(result, this);
		return result;
	}
	
} // class JSONObject
 
public static class ExJSONExtensions {
	
	public static string ToJson(this JSONObject obj) {
		return ExJSON.JsonEncode(obj);
	}
	
	public static JSONObject ToJsonObject(this string json) {
		return ExJSON.JsonDecode(json);
	}
	
} // class ExJSONExtensions
 
/// <summary>
/// This class encodes and decodes JSON strings.
/// Spec. details, see http://www.json.org/
///
/// JSON uses Arrays and Objects. These correspond here to the datatypes List<JSONObject> and Dictionary<string, JSONObject>.
/// All numbers are parsed to doubles.
/// </summary>
 
public class ExJSON {
	
	public const int TOKEN_NONE = 0;
	public const int TOKEN_CURLY_OPEN = 1;
	public const int TOKEN_CURLY_CLOSE = 2;
	public const int TOKEN_SQUARED_OPEN = 3;
	public const int TOKEN_SQUARED_CLOSE = 4;
	public const int TOKEN_COLON = 5;
	public const int TOKEN_COMMA = 6;
	public const int TOKEN_STRING = 7;
	public const int TOKEN_NUMBER = 8;
	public const int TOKEN_TRUE = 9;
	public const int TOKEN_FALSE = 10;
	public const int TOKEN_NULL = 11;
 
	private const int BUILDER_CAPACITY = 2000;
 
	/// <summary>
	/// Parses the string json into a value
	/// </summary>
	/// <param name="json">A JSON string.</param>
	/// <returns>An List<JSONObject>, a Dictionary<string, JSONObject>, a double, a string, null, true, or false</returns>
	public static JSONObject JsonDecode(string json) {
		bool success = true;
 
		return JsonDecode(json, ref success);
	}
 
	/// <summary>
	/// Parses the string json into a value; and fills 'success' with the successfullness of the parse.
	/// </summary>
	/// <param name="json">A JSON string.</param>
	/// <param name="success">Successful parse?</param>
	/// <returns>An List<JSONObject>, a Dictionary<string, JSONObject>, a double, a string, null, true, or false</returns>
	public static JSONObject JsonDecode(string json, ref bool success) {
		success = true;
		if (json != null) {
			char[] charArray = json.ToCharArray();
			int index = 0;
			JSONObject value = ParseValue(charArray, ref index, ref success);
			return value;
		} else {
			return null;
		}
	}
 
	/// <summary>
	/// Converts a Dictionary<string, JSONObject> / List<JSONObject> object into a JSON string
	/// </summary>
	/// <param name="json">A Dictionary<string, JSONObject> / List<JSONObject></param>
	/// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
	public static string JsonEncode(JSONObject json) {
		StringBuilder builder = new StringBuilder(BUILDER_CAPACITY);
		bool success = SerializeValue(json, builder);
		return (success ? builder.ToString() : null);
	}
 
	protected static /*Dictionary<string, JSONObject>*/JSONObject ParseObject(char[] json, ref int index, ref bool success) {
		Dictionary<string, JSONObject> table = new Dictionary<string, JSONObject>();
		int token;
 
		// {
		NextToken(json, ref index);
 
		bool done = false;
		while (!done) {
			token = LookAhead(json, index);
			if (token == ExJSON.TOKEN_NONE) {
				success = false;
				return null;
			} else if (token == ExJSON.TOKEN_COMMA) {
				NextToken(json, ref index);
			} else if (token == ExJSON.TOKEN_CURLY_CLOSE) {
				NextToken(json, ref index);
				return new JSONObject(table);
			} else {
 
				// name
				JSONObject name = ParseString(json, ref index, ref success);
				if (!success) {
					success = false;
					return null;
				}
 
				// :
				token = NextToken(json, ref index);
				if (token != ExJSON.TOKEN_COLON) {
					success = false;
					return null;
				}
 
				// value
				JSONObject value = ParseValue(json, ref index, ref success);
				if (!success) {
					success = false;
					return null;
				}
 
				table[(string)name] = value;
			}
		}
 
		return new JSONObject(table);
	}
 
	protected static JSONObject ParseArray(char[] json, ref int index, ref bool success) {
		List<JSONObject> array = new List<JSONObject>();
 
		// [
		NextToken(json, ref index);
 
		bool done = false;
		while (!done) {
			int token = LookAhead(json, index);
			if (token == ExJSON.TOKEN_NONE) {
				success = false;
				return null;
			} else if (token == ExJSON.TOKEN_COMMA) {
				NextToken(json, ref index);
			} else if (token == ExJSON.TOKEN_SQUARED_CLOSE) {
				NextToken(json, ref index);
				break;
			} else {
				JSONObject value = ParseValue(json, ref index, ref success);
				if (!success) {
					return null;
				}
 
				array.Add(value);
			}
		}
 
		return new JSONObject(array);
	}
 
	protected static JSONObject ParseValue(char[] json, ref int index, ref bool success) {
		switch (LookAhead(json, index)) {
			case ExJSON.TOKEN_STRING:
				return ParseString(json, ref index, ref success);
			case ExJSON.TOKEN_NUMBER:
				return ParseNumber(json, ref index, ref success);
			case ExJSON.TOKEN_CURLY_OPEN:
				return ParseObject(json, ref index, ref success);
			case ExJSON.TOKEN_SQUARED_OPEN:
				return ParseArray(json, ref index, ref success);
			case ExJSON.TOKEN_TRUE:
				NextToken(json, ref index);
				return new JSONObject(true);
			case ExJSON.TOKEN_FALSE:
				NextToken(json, ref index);
				return new JSONObject(false);
			case ExJSON.TOKEN_NULL:
				NextToken(json, ref index);
				return null;
			case ExJSON.TOKEN_NONE:
				break;
		}
 
		success = false;
		return null;
	}
 
	protected static JSONObject ParseString(char[] json, ref int index, ref bool success) {
		StringBuilder s = new StringBuilder(BUILDER_CAPACITY);
		char c;
 
		EatWhitespace(json, ref index);
 
		// "
		c = json[index++];
 
		bool complete = false;
		while (!complete) {
 
			if (index == json.Length) {
				break;
			}
 
			c = json[index++];
			if (c == '"') {
				complete = true;
				break;
			} else if (c == '\\') {
 
				if (index == json.Length) {
					break;
				}
				c = json[index++];
				if (c == '"') {
					s.Append('"');
				} else if (c == '\\') {
					s.Append('\\');
				} else if (c == '/') {
					s.Append('/');
				} else if (c == 'b') {
					s.Append('\b');
				} else if (c == 'f') {
					s.Append('\f');
				} else if (c == 'n') {
					s.Append('\n');
				} else if (c == 'r') {
					s.Append('\r');
				} else if (c == 't') {
					s.Append('\t');
				} else if (c == 'u') {
					int remainingLength = json.Length - index;
					if (remainingLength >= 4) {
						// parse the 32 bit hex into an integer codepoint
						uint codePoint;
						if (!(success = UInt32.TryParse(new string(json, index, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out codePoint))) {
							return new JSONObject("");
						}
						// convert the integer codepoint to a unicode char and add to string
						s.Append(Char.ConvertFromUtf32((int)codePoint));
						// skip 4 chars
						index += 4;
					} else {
						break;
					}
				}
 
			} else {
				s.Append(c);
			}
 
		}
 
		if (!complete) {
			success = false;
			return null;
		}
 
		return new JSONObject(s.ToString());
	}
 
	protected static JSONObject ParseNumber(char[] json, ref int index, ref bool success) {
		EatWhitespace(json, ref index);
 
		bool floating;
		int lastIndex = GetLastIndexOfNumber(json, index, out floating);
		int charLength = (lastIndex - index) + 1;
 
		JSONObject result;
		if (floating) {
			double number;
			success = Double.TryParse(new string(json, index, charLength), NumberStyles.Any, CultureInfo.InvariantCulture, out number);
			result = new JSONObject(number);
		} else {
			long number;
			success = long.TryParse(new string(json, index, charLength), NumberStyles.Any, CultureInfo.InvariantCulture, out number);
			result = new JSONObject(number);
		}
 
		index = lastIndex + 1;
		return result;
	}
 
	protected static int GetLastIndexOfNumber(char[] json, int index, out bool floating) {
		int lastIndex;
		floating = false;
 
		for (lastIndex = index; lastIndex < json.Length; lastIndex++) {
			int indexOf = "0123456789+-.eE".IndexOf(json[lastIndex]);
			if (indexOf == -1) {
				break;
			} else if (indexOf > 11) {
				floating = true;
			}
		}
		return lastIndex - 1;
	}
 
	protected static void EatWhitespace(char[] json, ref int index) {
		for (; index < json.Length; index++) {
			if (" \t\n\r".IndexOf(json[index]) == -1) {
				break;
			}
		}
	}
 
	protected static int LookAhead(char[] json, int index) {
		int saveIndex = index;
		return NextToken(json, ref saveIndex);
	}
 
	protected static int NextToken(char[] json, ref int index) {
		EatWhitespace(json, ref index);
 
		if (index == json.Length) {
			return ExJSON.TOKEN_NONE;
		}
 
		char c = json[index];
		index++;
		switch (c) {
			case '{':
				return ExJSON.TOKEN_CURLY_OPEN;
			case '}':
				return ExJSON.TOKEN_CURLY_CLOSE;
			case '[':
				return ExJSON.TOKEN_SQUARED_OPEN;
			case ']':
				return ExJSON.TOKEN_SQUARED_CLOSE;
			case ',':
				return ExJSON.TOKEN_COMMA;
			case '"':
				return ExJSON.TOKEN_STRING;
			case '0': case '1': case '2': case '3': case '4':
			case '5': case '6': case '7': case '8': case '9':
			case '-':
				return ExJSON.TOKEN_NUMBER;
			case ':':
				return ExJSON.TOKEN_COLON;
		}
		index--;
 
		int remainingLength = json.Length - index;
 
		// false
		if (remainingLength >= 5) {
			if (json[index] == 'f' &&
				json[index + 1] == 'a' &&
				json[index + 2] == 'l' &&
				json[index + 3] == 's' &&
				json[index + 4] == 'e') {
				index += 5;
				return ExJSON.TOKEN_FALSE;
			}
		}
 
		// true
		if (remainingLength >= 4) {
			if (json[index] == 't' &&
				json[index + 1] == 'r' &&
				json[index + 2] == 'u' &&
				json[index + 3] == 'e') {
				index += 4;
				return ExJSON.TOKEN_TRUE;
			}
		}
 
		// null
		if (remainingLength >= 4) {
			if (json[index] == 'n' &&
				json[index + 1] == 'u' &&
				json[index + 2] == 'l' &&
				json[index + 3] == 'l') {
				index += 4;
				return ExJSON.TOKEN_NULL;
			}
		}
 
		return ExJSON.TOKEN_NONE;
	}
 
	protected static bool SerializeValue(JSONObject value, StringBuilder builder) {
		bool success = true;
		
		if (value == null) {
			builder.Append("null");
		} else if (value.GetType() == typeof(string)) {
			success = SerializeString((string)value, builder);
		} else if (value.GetType() == typeof(Dictionary<string, JSONObject>)) {
			success = SerializeObject((Dictionary<string, JSONObject>)value, builder);
		} else if (value.GetType() == typeof(List<JSONObject>)) {
			success = SerializeArray((List<JSONObject>)value, builder);
		} else if ((value.GetType() == typeof(Boolean)) && ((Boolean)value == true)) {
			builder.Append("true");
		} else if ((value.GetType() == typeof(Boolean)) && ((Boolean)value == false)) {
			builder.Append("false");
		} else if (
			value.GetType() == typeof(double) || value.GetType() == typeof(long)
		) {
			success = SerializeNumber(value.ToDouble(), builder);
		} else {
			success = false;
		}
		return success;
	}
 
	protected static bool SerializeObject(Dictionary<string, JSONObject> anObject, StringBuilder builder) {
		builder.Append("\n{\n");
 
		bool first = true;
		foreach (KeyValuePair<string, JSONObject> kvp in anObject) {
			string key = kvp.Key;
			JSONObject value = kvp.Value;
 
			if (!first) {
				builder.Append(",\n");
			}
 
			SerializeString(key, builder);
			builder.Append(":");
			if (!SerializeValue(value, builder)) {
				return false;
			}
 
			first = false;
		}
 
		builder.Append("\n}\n");
		return true;
	}
 
	protected static bool SerializeArray(List<JSONObject> anArray, StringBuilder builder) {
		builder.Append("\n[\n");
		
		bool first = true;
		for (int i = 0; i < anArray.Count; i++) {
			JSONObject value = anArray[i];
			
			if (!first) {
				builder.Append(",\n");
			}
			
			if (!SerializeValue(value, builder)) {
				return false;
			}
			
			first = false;
		}
		
		builder.Append("\n]\n");
		return true;
	}
	
	protected static bool SerializeString(string aString, StringBuilder builder) {
		builder.Append("\"");
		
		char[] charArray = aString.ToCharArray();
		for (int i = 0; i < charArray.Length; i++) {
			char c = charArray[i];
			if (c == '"') {
				builder.Append("\\\"");
			} else if (c == '\\') {
				builder.Append("\\\\");
			} else if (c == '\b') {
				builder.Append("\\b");
			} else if (c == '\f') {
				builder.Append("\\f");
			} else if (c == '\n') {
				builder.Append("\\n");
			} else if (c == '\r') {
				builder.Append("\\r");
			} else if (c == '\t') {
				builder.Append("\\t");
			} else {
				int codepoint = Convert.ToInt32(c);
				if ((codepoint >= 32) && (codepoint <= 126)) {
					builder.Append(c);
				} else {
					builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
				}
			}
		}
		
		builder.Append("\"");
		return true;
	}
	
	protected static bool SerializeNumber(double number, StringBuilder builder) {
		builder.Append(Convert.ToString(number, CultureInfo.InvariantCulture));
		return true;
	}
}