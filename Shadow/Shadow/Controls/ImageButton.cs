using System;
using Xamarin.Forms;

namespace Shadow
{
	public class ImageButton : Frame
	{
		private Color _buttonBackgroundColor;
		private ImageSource _image;
		private double _imageWidthRequest;
		private double _imageHeightRequest;

		private Color _textColor;
		private double _fontSize;

		private Image _imageControl;
		private Label _lblText;
		private string _text;
		private ImageOrientation _ImageOrientation;
		private StackLayout _stackContainer;

		private double _imageTextPadding;

		public event EventHandler Clicked;

		public static BindableProperty CommandParameterProperty =
  			BindableProperty.Create("CommandParameter", typeof(string), typeof(ImageButton), "");

		public static BindableProperty TextBindingProperty =
  			BindableProperty.Create("TextBinding", typeof(string), typeof(ImageButton), "");

		public string CommandParameter
		{
			get { return (string)GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		public string TextBinding
		{
			get { return (string)GetValue(TextBindingProperty); }
			set { SetValue(TextBindingProperty, value); }
		}

		public string Text
		{
			get { return _text; }
			set { _text = value;  RedrawButton(); }
		}

		public ImageOrientation ImageOrientation
		{
			get { return _ImageOrientation; }
			set { _ImageOrientation = value; RedrawButton(); }
		}

		public double ImageWidthRequest
		{
			get { return _imageWidthRequest; }
			set { _imageWidthRequest = value; RedrawButton(); }
		}

		public double ImageHeightRequest
		{
			get { return _imageHeightRequest; }
			set { _imageHeightRequest = value; RedrawButton(); }
		}

		public double ImageTextPadding
		{
			get { return _imageTextPadding; }
			set { _imageTextPadding = value; RedrawButton(); }
		}

		public ImageSource Image
		{
			get { return _image; }
			set { _image = value; RedrawButton(); }
		}

		public Color TextColor
		{
			get { return _textColor; }
			set { _textColor = value; RedrawButton(); }
		}

		public double FontSize
		{
			get { return _fontSize; }
			set { _fontSize = value; RedrawButton(); }
		}


		public ImageButton()
		{
			Padding = 3; // Add a bit of padding from the frame for the rounded corner effect
			Margin = 1; // Space between this control and external controls
			HasShadow = false;

			_imageTextPadding = 0; // space between image and text

			_stackContainer = new StackLayout
			{
				//BackgroundColor = Xamarin.Forms.Color.Green,
				Spacing = 0,
				Padding = 3
			};
			Content = _stackContainer;

			//HorizontalOptions = LayoutOptions.FillAndExpand;
			//VerticalOptions = LayoutOptions.FillAndExpand;

			_lblText = new Label { 
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
			_imageControl = new Image {
				//BackgroundColor = Xamarin.Forms.Color.Red,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};

			_text = "";
			TextColor = Xamarin.Forms.Color.Black;
			_fontSize = 12;

			VerticalOptions = LayoutOptions.FillAndExpand;
			HorizontalOptions = LayoutOptions.FillAndExpand;

			var tapGestureRecognizer = new TapGestureRecognizer();
			tapGestureRecognizer.Tapped += (s, e) =>
			{
				ButtonClicked();
			};

			GestureRecognizers.Add(tapGestureRecognizer);

			Device.StartTimer(new TimeSpan(0, 0, 0, 0, 500), () =>
			{
				if (_lblText != null)
				{
					_lblText.Text = "";
					_lblText.IsVisible = false;
					_stackContainer.Children.Remove(_lblText);
					_lblText = null;
					_lblText = new Label
					{
						//BackgroundColor = _buttonBackgroundColor,
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.Center
					};
				}
				RedrawButton();
				return false;
			});

		}

		private void RedrawButton()
		{
			_stackContainer.Children.Clear();
			_stackContainer.HorizontalOptions = LayoutOptions.Center;
			_stackContainer.VerticalOptions = LayoutOptions.Center;
			_buttonBackgroundColor = BackgroundColor;

			StackLayout imageTextSpacer = new StackLayout
			{
				WidthRequest = 0,
				HeightRequest = 0
			};

			if (_ImageOrientation != null)
			{
				if ((_ImageOrientation == ImageOrientation.ImageOnTop) || (_ImageOrientation == ImageOrientation.ImageOnBottom))
					_stackContainer.Orientation = StackOrientation.Vertical;
				else
					_stackContainer.Orientation = StackOrientation.Horizontal;
			}

			_lblText.Text = _text;
			if (!string.IsNullOrEmpty(TextBinding))
				_lblText.Text = TextBinding;
			_lblText.HorizontalTextAlignment = TextAlignment.Center;
			_lblText.VerticalTextAlignment = TextAlignment.Center;
			_lblText.TextColor = TextColor;
			_lblText.FontSize = FontSize;

			_imageControl.Source = _image;
			_imageControl.WidthRequest = ImageWidthRequest;
			_imageControl.HeightRequest = ImageHeightRequest;

			if (_ImageOrientation == ImageOrientation.NoText)
			{
				if (_imageControl.Source != null)
					_stackContainer.Children.Add(_imageControl);
				_imageControl.HorizontalOptions = LayoutOptions.Center;
				_imageControl.VerticalOptions = LayoutOptions.Center;
			}
			else if (_ImageOrientation == ImageOrientation.NoImage)
			{
				if (!string.IsNullOrEmpty(_lblText.Text))
					_stackContainer.Children.Add(_lblText);
				_lblText.HorizontalOptions = LayoutOptions.Center;
				_lblText.VerticalOptions = LayoutOptions.Center;
			}
			else if ((_ImageOrientation == ImageOrientation.ImageOnTop) || (_ImageOrientation == ImageOrientation.ImageToLeft))
			{				
				if(_imageControl.Source != null)
					_stackContainer.Children.Add(_imageControl);
				if (!string.IsNullOrEmpty(_lblText.Text))
				{
					if(_ImageOrientation == ImageOrientation.ImageOnTop)
						imageTextSpacer.HeightRequest = _imageTextPadding;
					else
						imageTextSpacer.WidthRequest = _imageTextPadding;
					_stackContainer.Children.Add(imageTextSpacer);
					_stackContainer.Children.Add(_lblText);
				}
			}
			else if ((_ImageOrientation == ImageOrientation.ImageOnBottom) || (_ImageOrientation == ImageOrientation.ImageToRight))
			{
				if (!string.IsNullOrEmpty(_lblText.Text))
					_stackContainer.Children.Add(_lblText);
				if (_imageControl.Source != null)
				{
					if (_ImageOrientation == ImageOrientation.ImageOnBottom)
						imageTextSpacer.HeightRequest = _imageTextPadding;
					else
						imageTextSpacer.WidthRequest = _imageTextPadding;
					_stackContainer.Children.Add(imageTextSpacer);
					_stackContainer.Children.Add(_imageControl);
				}
			}
		}

		public void ButtonClicked()
		{
			EventHandler handler = Clicked;
			if (handler != null)
			{
				handler(this, new EventArgs());
			}
		}
	}

	public enum ImageOrientation
	{
		ImageToLeft = 0,
		ImageOnTop = 1,
		ImageToRight = 2,
		ImageOnBottom = 3,
		NoImage = 4,
		NoText = 5
		//ImageCentered = 4
	}
}