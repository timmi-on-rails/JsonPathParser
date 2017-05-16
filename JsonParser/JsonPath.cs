using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JsonParser
{
	public class JsonPathToken
	{
		public bool IsElementIndex { get; }

		public bool IsMemberName { get; }

		readonly int elementIndex;
		readonly string memberName;

		public JsonPathToken (string memberName)
		{
			if (memberName == null || memberName.Any (c => c < 'a' || c > 'z')) {
				throw new ArgumentException ("invalid membername " + memberName);
			}

			IsMemberName = true;
			this.memberName = memberName;
		}

		public JsonPathToken (int elementIndex)
		{
			if (elementIndex < 0) {
				throw new ArgumentException ("invalid element index");
			}

			IsElementIndex = true;
			this.elementIndex = elementIndex;
		}

		public int GetElementIndex ()
		{
			if (!IsElementIndex) {
				throw new ArgumentException ();
			}
			return elementIndex;

		}

		public string GetMemberName ()
		{
			if (!IsMemberName) {
				throw new ArgumentException ();
			}
			return memberName;
		}

		public override string ToString ()
		{
			if (IsMemberName) {
				return memberName;
			} else if (IsElementIndex) {
				return elementIndex.ToString ();
			}
			throw new ArgumentException ();
		}
	}

	public class JsonPath : IReadOnlyList<JsonPathToken>
	{
		readonly List<JsonPathToken> tokens;

		public JsonPath (IEnumerable<JsonPathToken> tokens)
		{
			this.tokens = tokens.ToList ();
		}

		public static JsonPath Parse (string path)
		{
			using (TextReader textReader = new StringReader (path)) {
				JsonPathParser parser = new JsonPathParser (textReader);
				return new JsonPath (parser.Scan ());
			}
		}

		public JsonPathToken this [int index] { get { return tokens [index]; } }

		public int Count { get { return tokens.Count; } }

		public IEnumerator<JsonPathToken> GetEnumerator ()
		{
			return tokens.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}
	}

	class JsonPathParser
	{
		readonly TextReader textReader;
		readonly StringBuilder buffer;

		int Peek () => textReader.Peek ();

		public JsonPathParser (TextReader textReader)
		{
			this.textReader = textReader;
			buffer = new StringBuilder ();
		}

		bool IsDigit ()
		{
			int next = Peek ();
			return '0' <= next && next <= '9';
		}

		bool IsLetter ()
		{
			int next = Peek ();
			return 'a' <= next && next <= 'z';
		}

		void Skip ()
		{
			textReader.Read ();
		}

		void Consume ()
		{
			buffer.Append ((char)textReader.Read ());
		}

		public IEnumerable<JsonPathToken> Scan ()
		{
			yield return ScanToken ();

			while (Peek () != -1) {
				if (Peek () == '.') {
					Skip ();
					yield return ScanToken ();
				} else {
					throw new ArgumentException ("unexpected char " + Peek ());
				}
			}
		}

		JsonPathToken ScanToken ()
		{
			if (IsDigit ()) {
				return ScanElementIndexToken ();
			} else if (IsLetter ()) {
				return ScanMemberNameToken ();
			} else {
				throw new ArgumentException ("expecting member name or element index, but not: " + (char)Peek ());
			}
		}

		JsonPathToken ScanElementIndexToken ()
		{
			while (IsDigit ()) {
				Consume ();
			}

			return new JsonPathToken (Convert.ToInt32 (CreateToken ()));
		}

		JsonPathToken ScanMemberNameToken ()
		{
			while (IsLetter ()) {
				Consume ();
			}

			return new JsonPathToken (CreateToken ());
		}

		string CreateToken ()
		{
			string value = buffer.ToString ();
			buffer.Clear ();
			return value;
		}
	}
}
