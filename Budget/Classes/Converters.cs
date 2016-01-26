using DevExpress.Xpf.Editors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace Budget {
    public class ObjectToTagConverter : MarkupExtension, IValueConverter {
        public ObjectToTagConverter() { }
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return value;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            ObservableCollection<Tag> lTag = new ObservableCollection<Tag>();
            List<object> lObj = value as List<object>;
            foreach (object o in lObj)
                lTag.Add((Tag)o);
            return lTag;

            //return value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }



    public class custConverter : MarkupExtension, IValueConverter {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return value;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }



        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }

    public class DatetoColorConverter : MarkupExtension, IValueConverter {
        public DatetoColorConverter() { }
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value is DateTime) {
                DateTime dt = (DateTime)value;
                TimeSpan t = (dt - new DateTime(2013, 1, 1));
                int days = (int)t.TotalDays + 1;
                var v = days - (int)(days / 3)*3;
                switch (v) {
                    case 0:
                        return new SolidColorBrush(Colors.CornflowerBlue);
                        
                    case 1:
                        return new SolidColorBrush(Colors.DarkSeaGreen);
                        
                   default:
                        return null;
                        
                }
            }
            return null;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return DateTime.Now;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
    public class DatetoColorConverter2 : MarkupExtension, IValueConverter {
        public DatetoColorConverter2() {
            DateDict = new Dictionary<DateTime, SolidColorBrush>();
        }
        Dictionary<DateTime, SolidColorBrush> DateDict;
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value is DateTime) {
                DateTime dt = (DateTime)value;
                if (DateDict.ContainsKey(dt)) {
                    return DateDict[dt];
                } else {
                    SolidColorBrush br;
                    var v = DateDict.Where(x => x.Key < dt).OrderBy(x => x.Key).ToList();
                    DateDict = v.ToDictionary(x => x.Key, x => x.Value);


                    if ( DateDict.Count == 0 || DateDict.Last().Value == null ) {
                        br = new SolidColorBrush(Colors.CornflowerBlue);
                    } else {
                        br = null;
                    }
                    DateDict.Add(dt, br);
                
                    return br;

                }
            }
            return null;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return DateTime.Now;
        }

        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }
 
    public class ConverterGroupDataToTitle : MarkupExtension, IValueConverter {
        public ConverterGroupDataToTitle() { }
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            GroupData gd = value as GroupData;
            if (gd != null) {
                string st = gd.ParentTagName + " - " + gd.Value.ToString("N0"); ;
                return st;
            }
            return value;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }



        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }

    public class ConverterIntToIsNegative : MarkupExtension, IValueConverter {
        public ConverterIntToIsNegative() { }
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (value == null) return false;
            int i = (int)value;
            return i < 0;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }



        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
    }

}
