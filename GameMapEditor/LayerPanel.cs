﻿using GameMapEditor.Frames;
using GameMapEditor.Objects;
using GameMapEditor.Objects.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace GameMapEditor
{
    public delegate void MapLayerAddedEventArgs(GameMapLayer layer);
    public delegate void MapLayerSelectionChangedEventArgs(int index);

    public partial class LayerPanel : DockContent, IDisposable
    {
        public event MapLayerAddedEventArgs MapLayerAdded;
        public event MapLayerSelectionChangedEventArgs MapLayerSelectionChanged;

        public LayerPanel()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            this.layerPanelCTM.Controls.Clear();
        }

        public void LoadFrom(GameMap map)
        {
            this.Clear();

            foreach (GameMapLayer layer in map.Layers)
            {
                this.layerPanelCTM.Add(layer);
            }
        }

        public void LoadLayer(GameMapLayer layer)
        {
            this.layerPanelCTM.Add(0, layer);
        }

        public void RemoveLayer(int index)
        {
            MapPanel mapPanel = DockPanel.ActiveDocument as MapPanel;
            if (mapPanel != null)
            {
                mapPanel.Map.RemoveLayerAt(index);
                this.layerPanelCTM.Remove(index);
            }
        }

        // TODO : Reviser
        private void ChangeLayerType(LayerControl sender)
        {
            if (this.layerPanelCTM.Controls.Count > 0)
            {
                MapPanel mapPanel = DockPanel.ActiveDocument as MapPanel;
                if (mapPanel != null)
                {
                    int index = this.layerPanelCTM.Controls.IndexOf(sender);
                    GameMapLayer layer = mapPanel.Map?.GetLayerAt(index);
                    if (layer != null)
                    {
                        layer.Type = layer.Type == LayerType.Lower ? LayerType.Upper : LayerType.Lower;
                        sender.Type = layer.Type;
                    }
                }
            }
        }

        // TODO : Reviser
        private void ChangeVisibleState(LayerControl sender)
        {
            if (this.layerPanelCTM.Controls.Count > 0)
            {
                MapPanel mapPanel = DockPanel.ActiveDocument as MapPanel;
                if (mapPanel != null)
                {
                    int index = this.layerPanelCTM.Controls.IndexOf(sender);
                    GameMapLayer layer = mapPanel.Map?.GetLayerAt(index);
                    if (layer != null)
                    {
                        layer.Visible = !layer.Visible;
                        sender.Visible = layer.Visible;
                    }
                }
            }
        }

        private void toolStripButtonAddLayer_Click(object sender, System.EventArgs e)
        {
            MapLayerFrame formular = new MapLayerFrame();
            formular.MapLayerAdded += Formular_MapLayerAdded;
            formular.ShowDialog();
            formular.MapLayerAdded -= Formular_MapLayerAdded;
        }


        private void toolStripButtonUpLayer_Click(object sender, EventArgs e)
        {
            if (this.layerPanelCTM.Controls.Count > 0)
            {
                int index1 = this.layerPanelCTM.SelectedIndex;
                MapPanel mapPanel = DockPanel.ActiveDocument as MapPanel;
                if (mapPanel != null && mapPanel.Map.SwapLayers(index1, index1 - 1))
                {
                    this.layerPanelCTM.Swap(index1, index1 - 1);
                }
            }
        }

        private void toolStripButtonDownLayer_Click(object sender, EventArgs e)
        {
            if(this.layerPanelCTM.Controls.Count > 0)
            {
                int index1 = this.layerPanelCTM.SelectedIndex;
                MapPanel mapPanel = DockPanel.ActiveDocument as MapPanel;
                if (mapPanel != null && mapPanel.Map.SwapLayers(index1, index1 + 1))
                {
                    this.layerPanelCTM.Swap(index1, index1 + 1);
                }
            }   
        }

        private void toolStripButtonRemoveLayer_Click(object sender, EventArgs e)
        {
            if (this.layerPanelCTM.Controls.Count > 0)
            {
                this.RemoveLayer(this.layerPanelCTM.SelectedIndex);
            }
        }

        private void toolStripButtonSetVisibleState_Click(object sender, EventArgs e)
        {
            ChangeVisibleState(sender as LayerControl);
        }

        private void Formular_MapLayerAdded(GameMapLayer layer)
        {
            RaiseLayerAddedEvent(layer);
        }

        private void RaiseLayerAddedEvent(GameMapLayer layer)
        {
            this.MapLayerAdded?.Invoke(layer);
        }

        private void RaiseLayerSelectionChangedEvent(int index)
        {
            this.MapLayerSelectionChanged?.Invoke(index);
        }

        private void layerPanelCTM_ItemSelectionChanged(object sender, int index)
        {
            this.RaiseLayerSelectionChangedEvent(index);
        }

        private void layerPanelCTM_LayerTypeChanged(object sender)
        {
            ChangeLayerType(sender as LayerControl);
        }

        private void layerPanelCTM_LayerVisibleStateChanged(object sender)
        {
            ChangeVisibleState(sender as LayerControl);
        }
    }
}
