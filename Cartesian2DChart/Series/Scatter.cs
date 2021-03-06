﻿using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Path = System.Windows.Shapes.Path;

namespace MvvmCharting
{


    /// <summary>
    /// This is used to display a point(dot) for an Item on the plotting area.
    /// On a Series, items scatters can be displayed on the line(curve).
    /// Each scatter represents an item, indicating there is an item point in
    /// this position.
    /// </summary>
    [ContentProperty(nameof(Data))]
    public class Scatter : Shape
    {


 
     
        /// <summary>
        /// Gets or sets a <see cref="T:System.Windows.Media.Geometry" /> that specifies the shape to be drawn.
        /// </summary>
        /// <returns>A description of the shape to be drawn. </returns>
        public Geometry Data
        {
            get
            {
                return (Geometry)this.GetValue(Scatter.DataProperty);
            }
            set
            {
                this.SetValue(Scatter.DataProperty, (object)value);
            }
        }
        public static readonly DependencyProperty DataProperty =
            Path.DataProperty.AddOwner(typeof(Scatter), new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        protected override Geometry DefiningGeometry
        {
            get
            {
                return this.Data ?? Geometry.Empty;
            }
        }

 

        public IScatterGeometryBuilder GeometryBuilder
        {
            get { return (IScatterGeometryBuilder)GetValue(GeometryBuilderProperty); }
            set { SetValue(GeometryBuilderProperty, value); }
        }
        public static readonly DependencyProperty GeometryBuilderProperty =
            DependencyProperty.Register("GeometryBuilder", typeof(IScatterGeometryBuilder), typeof(Scatter), new PropertyMetadata(null, OnGeometryBuilderPropertyChanged));



        private static void OnGeometryBuilderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scatter)d).UpdateScatterGeometry();
        }

        private bool _usingDefaultScatterGeometryBuilder = false;
        private void UpdateScatterGeometry()
        {
            if (this.GeometryBuilder == null)
            {
                return;
            }



            this.Data = this.GeometryBuilder?.GetGeometry();



        }

        public Scatter()
        {

            this.HorizontalAlignment = HorizontalAlignment.Left;
            this.VerticalAlignment = VerticalAlignment.Top;

            UpdateScatterGeometry();
        }



        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateActualPosition();
        }


        public Point Position
        {
            get { return (Point)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(Point), typeof(Scatter), new PropertyMetadata(PointHelper.EmptyPoint, OnPositionPropertyChanged));

        private static void OnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Scatter)d).OnPositionChanged((Point)e.NewValue);
        }
        private void OnPositionChanged(Point newValue)
        {
            UpdateActualPosition();


        }



        private void UpdateActualPosition()
        {
            if (this.Position.IsEmpty())
            {
                return;
            }

            var offset = GetOffsetForSizeChangedOverride(this.RenderSize);

            if (offset.IsEmpty())
            {
                return;
            }

            var x = this.Position.X + offset.X;
            var y = this.Position.Y + offset.Y;
            var translateTransform = this.RenderTransform as TranslateTransform;
            if (translateTransform == null)
            {
                TranslateTransform a = new TranslateTransform(x, y);
                this.RenderTransform = a;
            }
            else
            {
                translateTransform.Y = y;
                translateTransform.X = x;
            }
        }


        protected virtual Point GetOffsetForSizeChangedOverride(Size newSize)
        {
            return new Point(-newSize.Width / 2, -newSize.Height / 2);
        }
    }




}