using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Biaui.Showcase;

public class NotificationObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private static readonly ConcurrentDictionary<string, PropertyChangedEventArgs> PropChanged = new();

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;

        storage = value;

        if (PropertyChanged is not null)
            RaisePropertyChanged(propertyName);

        return true;
    }

    protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
        if (PropertyChanged is null)
            return;

        var pc = PropChanged.GetOrAdd(propertyName, name => new PropertyChangedEventArgs(name));

        PropertyChanged.Invoke(this, pc);
    }
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

    public string Name { get; set; } = "";
    public List<Person> Child { get; set; } = new ();

    public long Data1 { get; set; } = _count ++;
    public string Data2 { get; set; } = Guid.NewGuid().ToString();
}

public class Data : NotificationObject
{
    public Area[] Areas { get; } =
    {
        new (0, "北海道"),
        new (1, "青森県"),
        new (2, "岩手県"),
        new (3, "宮城県"),
        new (4, "秋田県"),
        new (5, "山形県"),
        new (6, "福島県"),
        new (7, "茨城県"),
        new (8, "栃木県"),
        new (9, "群馬県"),
        new (10, "埼玉県"),
        new (11, "千葉県"),
        new (12, "東京都"),
        new (13, "神奈川県"),
        new (14, "新潟県"),
        new (15, "富山県"),
        new (16, "石川県"),
        new (17, "福井県"),
        new (18, "山梨県"),
        new (19, "長野県"),
        new (20, "岐阜県"),
        new (21, "静岡県"),
        new (22, "愛知県"),
        new (23, "三重県"),
        new (24, "滋賀県"),
        new (25, "京都府"),
        new (26, "大阪府"),
        new (27, "兵庫県"),
        new (28, "奈良県"),
        new (29, "和歌山県"),
        new (30, "鳥取県"),
        new (31, "島根県"),
        new (32, "岡山県"),
        new (33, "広島県"),
        new (34, "山口県"),
        new (35, "徳島県"),
        new (36, "香川県"),
        new (37, "愛媛県"),
        new (38, "高知県"),
        new (39, "福岡県"),
        new (40, "佐賀県"),
        new (41, "長崎県"),
        new (42, "熊本県"),
        new (43, "大分県"),
        new (44, "宮崎県"),
        new (45, "鹿児島県"),
        new (46, "沖縄県")
    };

    public List<Person> Persons { get; }

    public ObservableCollection<string> ShortItems { get; }

    public ObservableCollection<string> LongItems { get; }

    #region SelectedShortItem

    private string _SelectedShortItem = "";

    public string SelectedShortItem
    {
        get => _SelectedShortItem;
        set => SetProperty(ref _SelectedShortItem, value);
    }

    #endregion

    #region SelectedLongItem

    private string _SelectedLongItem = "";

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