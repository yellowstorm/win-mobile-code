﻿/*
* Copyright 2007 ZXing authors
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
namespace com.google.zxing.common
{
    using System;
    using MonochromeBitmapSource = com.google.zxing.MonochromeBitmapSource;
    using ReaderException = com.google.zxing.ReaderException;
    /// <summary> Implementations of this class can, given locations of finder patterns for a QR code in an
    /// image, sample the right points in the image to reconstruct the QR code, accounting for
    /// perspective distortion. It is abstracted since it is relatively expensive and should be allowed
    /// to take advantage of platform-specific optimized implementations, like Sun's Java Advanced
    /// Imaging library, but which may not be available in other environments such as J2ME, and vice
    /// versa.
    /// *
    /// The implementation used can be controlled by calling {@link #setGridSampler(GridSampler)}
    /// with an instance of a class which implements this interface.
    /// *
    /// </summary>
    /// <author>  Sean Owen
    /// 
    /// </author>
    public abstract class GridSampler
    {
        public static GridSampler Instance
        {
            get
            {
                // No real point in trying to make this thread-safe;
                // doesn't matter if a second instance is created
                if (gridSampler == null)
                {
                    gridSampler = new DefaultGridSampler();
                }
                return gridSampler;
            }

        }

        private static GridSampler gridSampler = null;
        /// <summary> <p>Samples an image for a square matrix of bits of the given dimension. This is used to extract the
        /// black/white modules of a 2D barcode like a QR Code found in an image. Because this barcode may be
        /// rotated or perspective-distorted, the caller supplies four points in the source image that define
        /// known points in the barcode, so that the image may be sampled appropriately.</p>
        /// *
        /// <p>The last eight "from" parameters are four X/Y coordinate pairs of locations of points in
        /// the image that define some significant points in the image to be sample. For example,
        /// these may be the location of finder pattern in a QR Code.</p>
        /// *
        /// <p>The first eight "to" parameters are four X/Y coordinate pairs measured in the destination
        /// {@link BitMatrix}, from the top left, where the known points in the image given by the "from" parameters
        /// map to.</p>
        /// *
        /// <p>These 16 parameters define the transformation needed to sample the image.</p>
        /// *
        /// </summary>
        /// <param name="image">image to sample
        /// </param>
        /// <param name="dimension">width/height of {@link BitMatrix} to sample from iamge
        /// </param>
        /// <returns> {@link BitMatrix} representing a grid of points sampled from the image within a region
        /// defined by the "from" parameters
        /// @throws ReaderException if image can't be sampled, for example, if the transformation defined by
        /// the given points is invalid or results in sampling outside the image boundaries
        /// 
        /// </returns>
        public abstract BitMatrix sampleGrid(MonochromeBitmapSource image, int dimension, float p1ToX, float p1ToY, float p2ToX, float p2ToY, float p3ToX, float p3ToY, float p4ToX, float p4ToY, float p1FromX, float p1FromY, float p2FromX, float p2FromY, float p3FromX, float p3FromY, float p4FromX, float p4FromY);

        /// <summary> <p>Checks a set of points that have been transformed to sample points on an image against
        /// the image's dimensions to see if the point are even within the image.</p>
        /// *
        /// <p>This method will actually "nudge" the endpoints back onto the image if they are found to be barely
        /// (less than 1 pixel) off the image. This accounts for imperfect detection of finder patterns in an image
        /// where the QR Code runs all the way to the image border.</p>
        /// *
        /// <p>For efficiency, the method will check points from either end of the line until one is found
        /// to be within the image. Because the set of points are assumed to be linear, this is valid.</p>
        /// *
        /// </summary>
        /// <param name="image">image into which the points should map
        /// </param>
        /// <param name="points">actual points in x1,y1,...,xn,yn form
        /// @throws ReaderException if an endpoint is lies outside the image boundaries
        /// 
        /// </param>
        protected internal static void checkAndNudgePoints(MonochromeBitmapSource image, float[] points)
        {
            int width = image.getWidth();
            int height = image.getHeight();
            // Check and nudge points from start until we see some that are OK:
            bool nudged = true;
            for (int offset = 0; offset < points.Length && nudged; offset += 2)
            {
                int x = (int)points[offset];
                int y = (int)points[offset + 1];
                if (x < -1 || x > width || y < -1 || y > height)
                {
                    throw new ReaderException("");
                }
                nudged = false;
                if (x == -1)
                {
                    points[offset] = 0.0f;
                    nudged = true;
                }
                else if (x == width)
                {
                    points[offset] = width - 1;
                    nudged = true;
                }
                if (y == -1)
                {
                    points[offset + 1] = 0.0f;
                    nudged = true;
                }
                else if (y == height)
                {
                    points[offset + 1] = height - 1;
                    nudged = true;
                }
            }
            // Check and nudge points from end:
            nudged = true;
            for (int offset = points.Length - 2; offset >= 0 && nudged; offset -= 2)
            {
                int x = (int)points[offset];
                int y = (int)points[offset + 1];
                if (x < -1 || x > width || y < -1 || y > height)
                {
                    throw new ReaderException("");
                }
                nudged = false;
                if (x == -1)
                {
                    points[offset] = 0.0f;
                    nudged = true;
                }
                else if (x == width)
                {
                    points[offset] = width - 1;
                    nudged = true;
                }
                if (y == -1)
                {
                    points[offset + 1] = 0.0f;
                    nudged = true;
                }
                else if (y == height)
                {
                    points[offset + 1] = height - 1;
                    nudged = true;
                }
            }
        }
    }
}