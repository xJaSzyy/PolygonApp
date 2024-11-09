<template>
  <div id="app">
    <PolygonEditor />
  </div>
</template>

<script>
import PolygonEditor from './components/PolygonEditor.vue';

export default {
  name: 'App',
  components: {
    PolygonEditor,
  },
};
</script>

<style>
body, html, #app {
  margin: 0;
  height: 100%;
  font-family: Arial, sans-serif;
}
</style>
