using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace VostokVR.Geo {

    [System.Serializable]
    public class EffectiveText {

        private static StringBuilder _string = new StringBuilder( 100 );

        [SerializeField]
        [Tooltip( "Reference to te text field of UI <Text> component" )]
	    private Text text;

        [SerializeField]
        [Tooltip( "Length of internal buffer of string; default = 100" )]
        [Range( 100, 5000 )]
	    private int _capacity = 100;

	    private StringBuilder _builder;

        public void SetColor( Color color ) { text.color = color; }
        public void SetActive( bool state ) { text.enabled = state; }

        public Text Text_component { get { return text; } }
        public void SetTextComponent( Text text_component ) { text = text_component; }

        public Color Get_color { get { return text.color; } }
        public string Text_field { get { return text.text; } }
        public string Builder_string { get { return _builder.ToString(); } }
        public bool Empty { get { return (text == null); } }
        public int Length { get { return (text == null) ? 0 : text.text.Length; } }

        public EffectiveText RewriteSeparatedInt( int number ) { Clear(); return AppendSeparatedInt( number ); }
        public EffectiveText RewriteDottedFloat( float number, int decimal_signs = 1 ) { Clear(); return AppendDottedFloat( number, decimal_signs ); }
        public EffectiveText RewriteSeparatedFloat( float number, int decimal_signs = 1 ) { Clear(); return AppendSeparatedFloat( number, decimal_signs ); }

        /// <summary>
        /// Returns digit as string.
        /// <summry/>
        /// ################################################################################################################################################################################################################################################
        public static string FloatToString( float digit, int signs = 2, string point = ".", string separator = "'" ) {

            string result = digit.ToString();
            string whole_part = string.Empty;
            string divisional_part = string.Empty;

            int digits_count = 0;
            int point_index = -1;

            if( result.IndexOf( "." ) != -1 ) {

                point_index = result.IndexOf( "." );
            }

            else if( result.IndexOf( "," ) != -1 ) {

                point_index = result.IndexOf( "," );
            }

            if( point_index < 0 ) {

                whole_part = result;
                
                if( signs > 0 ) {

                    divisional_part += point;

                    for( int i = 0; i < signs; i++ ) divisional_part += "0";
                }
            }

            else {

                whole_part = (point_index == 0) ? "0" : result.Substring( 0, point_index );

                if( signs > 0 ) {

                    divisional_part += point;

                    for( int i = point_index + 1; (i < result.Length) && (digits_count < signs); i++, digits_count++ ) divisional_part += result.Substring( i, 1 );

                    for( ; digits_count < signs; digits_count++ ) divisional_part += "0";
                }
            }

            return whole_part + divisional_part;
        }

        // Формирует строку для целого числа, в которой отделены разряды по тысячам ################################################################################################
        public EffectiveText AppendSeparatedInt( int number ) {

 		    if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );

            _string.Length = 0;
            _string.Append( number );

            for( int i = _string.Length - 1, count = 0, pos = i; i >= 0; i--, pos-- ) {

                if( (++count == 3) && (i > 0) ) {

                    count = 0;
                    _string.Insert( pos, " " );
                }
            }

            _builder.Append( _string.ToString() );

            text.text = _builder.ToString();

            return this;
        }

        // Формирует строку для числа с плавающей запятой, в которой разряды отделены по тысячам ###################################################################################
        public EffectiveText AppendSeparatedFloat( float number, int decimal_signs = 1 ) {

		    if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );

            AppendSeparatedInt( Mathf.FloorToInt( number ) );
            AppendDottedFloat( (number - Mathf.Floor( number )), decimal_signs, false );

            text.text = _builder.ToString();

            return this;
        }
    
        // Формирует строку для числа с плавающей запятой без разделения разрядов ##################################################################################################
        public EffectiveText AppendDottedFloat( float number, int decimal_signs = 1, bool use_integer_part = true ) {

            if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );

            if( use_integer_part ) {

                Append( Mathf.FloorToInt( number ) );
                number -= Mathf.Floor( number );
            }

            if( decimal_signs > 0 ) Append( "." );

            for( int i = 0; i < decimal_signs; i++ ) {

                number *= 10f;
                Append( Mathf.FloorToInt( number ) );
                number -= Mathf.Floor( number );
            }

            text.text = _builder.ToString();

            return this;
        }

        // #########################################################################################################################################################################
        public static string MakeSeparatedInt( int number, string separator = "," ) {

            _string.Length = 0;
            _string.Append( number );

            for( int i = _string.Length - 1, count = 0, pos = i; i >= 0; i--, pos-- ) {

                if( (++count == 3) && (i > 0) ) {

                    count = 0;
                    _string.Insert( pos, separator );
                }
            }

            return _string.ToString();
        }
    
        // #########################################################################################################################################################################
        public static string MakeSeparatedFloat( float number, int decimal_signs = 2, string separator = ",", string point = "." ) {

            _string.Length = 0;
            _string.Append( Mathf.FloorToInt( number ) );

            for( int i = _string.Length - 1, count = 0, pos = i; i >= 0; i--, pos-- ) {

                if( (++count == 3) && (i > 0) ) {

                    count = 0;
                    _string.Insert( pos, separator );
                }
            }

            _string.Append( point );

            float rest = Mathf.Abs( number - Mathf.Floor( number ) );

            for( int i = 0; i < decimal_signs; i++ ) {

                rest *= 10f;

                if( Mathf.FloorToInt( rest ) > 0 ) _string.Append( Mathf.FloorToInt( rest ) );
                else _string.Append( "0" );
            }

            return _string.ToString();
        }
    
        // #########################################################################################################################################################################
	    public EffectiveText Rewrite( int _value ) {

		    if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );
		
		    _builder.Length = 0;
		    _builder.Append( _value );

            text.text = _builder.ToString();

            return this;
	    }

        // #########################################################################################################################################################################
        public EffectiveText Rewrite( float _value ) {

		    if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );
		
		    _builder.Length = 0;
		    _builder.Append( _value );

            text.text = _builder.ToString();

            return this;
	    }

        // #########################################################################################################################################################################
        public EffectiveText Rewrite( string _str ) {

            if( _str == null ) return this;
		    if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );
            if( _capacity < _str.Length ) _builder = new StringBuilder( _str, (_capacity = _str.Length * 2) );
		
		    _builder.Length = 0;
		    _builder.Append( _str );

            text.text = _builder.ToString();

            return this;
	    }

        // #########################################################################################################################################################################
        public EffectiveText Append( int _value ) {

		    if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );
            if( _capacity < (_builder.Length + 10) ) _builder = new StringBuilder( text.text, (_capacity *= 2) );

		    _builder.Append( _value );

            text.text = _builder.ToString();

            return this;
	    }

        // #########################################################################################################################################################################
        public EffectiveText Append( float _value ) {

		    if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );
            if( _capacity < (_builder.Length + 10) ) _builder = new StringBuilder( text.text, (_capacity *= 2) );

		    _builder.Append( _value );

            text.text = _builder.ToString();

            return this;
	    }

        // #########################################################################################################################################################################
        public EffectiveText Append( string _str ) {

            if( _str == null ) return this;
		    if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );
            if( _capacity < (_builder.Length + _str.Length) ) _builder = new StringBuilder( text.text, (_capacity = (_builder.Length + _str.Length) * 2) );

		    _builder.Append( _str );

            text.text = _builder.ToString();

            return this;
	    }

        // #########################################################################################################################################################################
        public EffectiveText Clear() {

		    if( _builder == null ) _builder = new StringBuilder( _capacity, _capacity );

            _builder.Length = 0;

            text.text = _builder.ToString();

            return this;
        }
    }
}