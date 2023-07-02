using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Biaui.Showcase;

public class NotificationObject : INotifyPropertyChanged
{
    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;

        storage = value;

        RaisePropertyChanged(propertyName);

        return true;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
        // ReSharper disable once InconsistentlySynchronizedField
        var pc = (PropertyChangedEventArgs)_propChanged[propertyName];

        if (pc is null)
        {
            // double-checked;
            lock (_propChanged)
            {
                pc = (PropertyChangedEventArgs)_propChanged[propertyName];

                if (pc is null)
                {
                    pc = new PropertyChangedEventArgs(propertyName);
                    _propChanged[propertyName] = pc;
                }
            }
        }

        PropertyChanged?.Invoke(this, pc);
    }

    // use Hashtable to get free lockless reading
    private static readonly Hashtable _propChanged = new Hashtable();
}

public class Area
{
    public Area(int index, string name)
    {
        Index = index;
        Name = name;
    }


    public int Index { get; set; }
    public string Name { get; set; }
}

public class Person
{
    private static int _count;

    public string Name { get; set; }
    public List<Person> Child { get; set; }

    public long Data1 { get; set; } = _count ++;
    public string Data2 { get; set; } = Guid.NewGuid().ToString();
}

public class Data : NotificationObject
{
    public Area[] Areas { get; } =
    {
        new Area(0, "北海道"),
        new Area(1, "青森県"),
        new Area(2, "岩手県"),
        new Area(3, "宮城県"),
        new Area(4, "秋田県"),
        new Area(5, "山形県"),
        new Area(6, "福島県"),
        new Area(7, "茨城県"),
        new Area(8, "栃木県"),
        new Area(9, "群馬県"),
        new Area(10, "埼玉県"),
        new Area(11, "千葉県"),
        new Area(12, "東京都"),
        new Area(13, "神奈川県"),
        new Area(14, "新潟県"),
        new Area(15, "富山県"),
        new Area(16, "石川県"),
        new Area(17, "福井県"),
        new Area(18, "山梨県"),
        new Area(19, "長野県"),
        new Area(20, "岐阜県"),
        new Area(21, "静岡県"),
        new Area(22, "愛知県"),
        new Area(23, "三重県"),
        new Area(24, "滋賀県"),
        new Area(25, "京都府"),
        new Area(26, "大阪府"),
        new Area(27, "兵庫県"),
        new Area(28, "奈良県"),
        new Area(29, "和歌山県"),
        new Area(30, "鳥取県"),
        new Area(31, "島根県"),
        new Area(32, "岡山県"),
        new Area(33, "広島県"),
        new Area(34, "山口県"),
        new Area(35, "徳島県"),
        new Area(36, "香川県"),
        new Area(37, "愛媛県"),
        new Area(38, "高知県"),
        new Area(39, "福岡県"),
        new Area(40, "佐賀県"),
        new Area(41, "長崎県"),
        new Area(42, "熊本県"),
        new Area(43, "大分県"),
        new Area(44, "宮崎県"),
        new Area(45, "鹿児島県"),
        new Area(46, "沖縄県")
    };

    public List<Person> Persons { get; }

    #region ShortItems

    private ObservableCollection<string> _ShortItems;

    public ObservableCollection<string> ShortItems
    {
        get => _ShortItems;
        set => SetProperty(ref _ShortItems, value);
    }

    #endregion

    #region LongItems

    private ObservableCollection<string> _LongItems;

    public ObservableCollection<string> LongItems
    {
        get => _LongItems;
        set => SetProperty(ref _LongItems, value);
    }

    #endregion

    #region SelectedShortItem

    private string _SelectedShortItem;

    public string SelectedShortItem
    {
        get => _SelectedShortItem;
        set => SetProperty(ref _SelectedShortItem, value);
    }

    #endregion

    #region SelectedLongItem

    private string _SelectedLongItem;

    public string SelectedLongItem
    {
        get => _SelectedLongItem;
        set => SetProperty(ref _SelectedLongItem, value);
    }

    #endregion


    public Data()
    {
        ShortItems = new ObservableCollection<string>
        {
            "あいうえお",
            "将棋",
            "東京",
            "大阪",
            "カード",
            "スピーカー"
        };

        LongItems = new ObservableCollection<string>
        {
            "北海道",
            "青森県",
            "岩手県",
            "宮城県",
            "秋田県",
            "山形県",
            "福島県",
            "茨城県",
            "栃木県",
            "群馬県",
            "埼玉県",
            "千葉県",
            "東京都",
            "神奈川県",
            "新潟県",
            "富山県",
            "石川県",
            "福井県",
            "山梨県",
            "長野県",
            "岐阜県",
            "静岡県",
            "愛知県",
            "三重県",
            "滋賀県",
            "京都府",
            "大阪府",
            "兵庫県",
            "奈良県",
            "和歌山県",
            "鳥取県",
            "島根県",
            "岡山県",
            "広島県",
            "山口県",
            "徳島県",
            "香川県",
            "愛媛県",
            "高知県",
            "福岡県",
            "佐賀県",
            "長崎県",
            "熊本県",
            "大分県",
            "宮崎県",
            "鹿児島県",
            "沖縄県"
        };

        SelectedShortItem = ShortItems[0];
        SelectedLongItem = LongItems[0];

        Persons = new List<Person>()
        {
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },

            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },


            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },


            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },

            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },

            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },


            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },


            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },

            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },

            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },


            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },


            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },

            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },

            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },


            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },


            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
            new Person()
            {
                Name = "test1",
                Child = new List<Person>()
                {
                    new Person() {Name = "test1-1"},
                    new Person() {Name = "test1-2"},
                    new Person() {Name = "test1-3"},
                }
            },
            new Person()
            {
                Name = "test2",
                Child = new List<Person>()
                {
                    new Person() {Name = "test2-1"},
                    new Person()
                    {
                        Name = "test2-2",
                        Child = new List<Person>()
                        {
                            new Person() {Name = "test2-2-1"},
                            new Person() {Name = "test2-2-2"}
                        }
                    }
                }
            },
        };
    }
}