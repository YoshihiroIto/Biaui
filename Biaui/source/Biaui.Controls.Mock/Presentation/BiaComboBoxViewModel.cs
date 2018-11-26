using System.Collections.ObjectModel;
using Biaui.Controls.Mock.Foundation.Interface;
using Biaui.Controls.Mock.Foundation.Mvvm;

namespace Biaui.Controls.Mock.Presentation
{
    public class BiaComboBoxViewModel : ViewModelBase
    {
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

        public BiaComboBoxViewModel(IDisposableChecker disposableChecker) : base(disposableChecker)
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
        }
    }
}