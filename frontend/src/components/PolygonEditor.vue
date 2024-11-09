<template>
    <div id="map" ref="mapContainer" style="height: 100%; width: 100%"></div>
    
    <div class="response-panel" :style="{ color: responseTextColor }">
      <pre>{{ responseData }}</pre>
    </div>
</template>

<script>
import L from "leaflet";
import "leaflet-draw";
import axios from "axios";

export default {
  name: "PolygonEditor",
  data() {
    return {
      isDrawing: false,
      clickCoordinates: { latitude: 0, longitude: 0 },
      polygons: [],
      responseData: null, 
      responseTextColor: "white",
    };
  },
  mounted() {
    this.map = L.map(this.$refs.mapContainer).setView(
      [55.751244, 37.618423],
      10
    );

    L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
      maxZoom: 18,
    }).addTo(this.map);

    this.drawControl = new L.Control.Draw({
      draw: {
        polygon: true,
        polyline: false,
        rectangle: false,
        circle: false,
        circlemarker: false,
        marker: false,
      },
    });
    this.map.addControl(this.drawControl);

    this.map.on(L.Draw.Event.DRAWSTART, () => {
      this.isDrawing = true;
    });
    this.map.on(L.Draw.Event.DRAWSTOP, () => {
      this.isDrawing = false;
    });

    this.map.on(L.Draw.Event.CREATED, (event) => {
      const { layer } = event;
      this.polygons.push(layer); 
      layer.addTo(this.map);
    });

    this.map.on("click", async (event) => {
      if (!this.isDrawing) {
        const { lat, lng } = event.latlng;

        if (this.polygons.length > 0) {
          const polygonCoordinates = this.polygons.map((polygon) =>
            polygon.getLatLngs()[0].map((latlng) => ({
              latitude: latlng.lat,
              longitude: latlng.lng,
            }))
          );

          try {
            const response = await axios.post(
              "http://localhost:5000/api/Polygon/polygon/checkPoint",
              {
                point: { latitude: lat, longitude: lng },
                polygons: polygonCoordinates.map(coords => ({ Points: coords }))
              }
            );

            if (response.data.isPointInside) {
              this.responseData = "Точка внутри полигона";
              this.responseTextColor = "green"; 
            } else {
              this.responseData = "Точка вне полигона";
              this.responseTextColor = "red"; 
            }
          } catch (error) {
            console.error("Ошибка при проверке точки:", error);
          }
        } else {
          this.responseData = "Полигонов нет";
        }
      }
    });
  },
};
</script>

<style scoped>
#map {
  height: 100vh;
}

.response-panel {
  position: fixed;
  bottom: 0;
  left: 0;
  width: 100%;
  background-color: rgba(0, 0, 0, 0.7);
  color: white;
  padding: 10px;
  font-family: monospace;
  font-size: 14px;
  z-index: 999;
  overflow: auto;
  max-height: 200px;
}
</style>
