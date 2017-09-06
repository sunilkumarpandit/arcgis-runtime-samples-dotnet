// Copyright 2017 Esri.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific
// language governing permissions and limitations under the License.

using Android.App;
using Android.OS;
using Android.Widget;
using ArcGISRuntimeXamarin.Managers;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI.Controls;
using System.IO;
using System.Threading.Tasks;

namespace ArcGISRuntimeXamarin.Samples.FeatureLayerDictionaryRenderer
{
    [Activity]
    public class FeatureLayerDictionaryRenderer : Activity
    {
        // Create and hold reference to the used MapView
        private MapView _myMapView = new MapView();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Title = "Feature layer dictionary renderer";

            // Create the UI, setup the control references and execute initialization
            CreateLayout();
            Initialize();
        }

        private void CreateLayout()
        {
            // Create a new vertical layout for the app
            var layout = new LinearLayout(this) { Orientation = Orientation.Vertical };

            // Add the map view to the layout
            layout.AddView(_myMapView);

            // Show the layout in the app
            SetContentView(layout);
        }

        private async void Initialize()
        {
            // Create new Map with basemap
            Map myMap = new Map(Basemap.CreateTopographic());

            // Provide Map to the MapView
            _myMapView.Map = myMap;

            // Create geometry for the center of the map
            MapPoint centerGeometry = new MapPoint(-13549402.587055, 4397264.96879385, SpatialReference.Create(3857));

            // Set the map's viewpoint to highlight the desired content
            await _myMapView.SetViewpointCenterAsync(centerGeometry);
            await _myMapView.SetViewpointScaleAsync(201555.279);

            // Get the path to the geodatabase
            string geodbFilePath = await GetGeodatabasePath();

            // Load the geodatabase from local storage
            Geodatabase baseGeodatabase = await Geodatabase.OpenAsync(geodbFilePath);

            // Get the path to the symbol dictionary
            string symbolFilepath = await GetStyleDictionaryPath();

            // Load the symbol dictionary from local storage
            //     Note that the type of the symbol definition must be explicitly provided along with the file name
            DictionarySymbolStyle symbolStyle = await DictionarySymbolStyle.OpenAsync("mil2525d", symbolFilepath);

            // Add geodatabase features to the map, using the defined symbology
            foreach (FeatureTable table in baseGeodatabase.GeodatabaseFeatureTables)
            {
                // Load the table
                await table.LoadAsync();

                // Create the feature layer from the table
                FeatureLayer myLayer = new FeatureLayer(table);

                // Load the layer
                await myLayer.LoadAsync();

                // Create a Dictionary Renderer using the DictionarySymbolStyle
                DictionaryRenderer dictRenderer = new DictionaryRenderer(symbolStyle);

                // Apply the dictionary renderer to the layer
                myLayer.Renderer = dictRenderer;

                // Add the layer to the map
                myMap.OperationalLayers.Add(myLayer);
            }
        }

        // Get the file path for the style dictionary
        private async Task<string> GetStyleDictionaryPath()
        {
            #region offlinedata
            // The data manager provides a method to get the folder
            string folder = DataManager.GetDataFolder();

            // Get the full path
            string filepath = Path.Combine(folder, "SampleData", "FeatureLayerDictionaryRenderer", "mil2525d.stylx");

            // Check if the file exists
            if (!File.Exists(filepath))
            {
                // Download the file
                await DataManager.GetData("e34835bf5ec5430da7cf16bb8c0b075c", "FeatureLayerDictionaryRenderer");
            }

            return filepath;
            #endregion offlinedata
        }

        // Get the file path for the geodatabase
        private async Task<string> GetGeodatabasePath()
        {
            #region offlinedata
            // The data manager provides a method to get the folder
            string folder = DataManager.GetDataFolder();

            // Get the full path
            string filepath = Path.Combine(folder, "SampleData", "FeatureLayerDictionaryRenderer", "militaryoverlay.geodatabase");

            // Check if the file exists
            if (!File.Exists(filepath))
            {
                // Download the file
                await DataManager.GetData("e0d41b4b409a49a5a7ba11939d8535dc", "FeatureLayerDictionaryRenderer");
            }

            return filepath;
            #endregion offlinedata
        }
    }
}