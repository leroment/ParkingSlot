module.exports = {
  configureWebpack: {
  },
  devServer: {
    host: '0.0.0.0',
    hot: true,
    port: 8080,
    https: true,
    open: 'Chrome',
    disableHostCheck: true,
  }
}
