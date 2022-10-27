const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');

let mode = 'development';
if (process.env.NODE_ENV === 'production')
    mode = 'production';
console.log(`mode: ${mode} (NOW NOT EXPORTING, uncomment that if needed.)`);

module.exports = {
    //mode: mode,
    entry: {
        dictionary: path.resolve(__dirname, './src/dictionary-index.js')
    },
    output: {
        path: path.resolve(__dirname, './dist'),
        filename: 'script/[name].[contenthash].js',
        clean: true
    },
    module: {
        rules: [
            {
                test: /\.(js|jsx)$/,
                exclude: /node_modules/,
                use: ["babel-loader"]
            },
            {
                test: /\.(sc|sa|c)ss$/,
                exclude: /node_modules/,
                use: [
                    //? Тут, видимо, чтобы css-ки выгружались, нужно будет MiniCssExtractPlugin.loader - ну и не забыть в plugins добавить new...
                    'style-loader',
                    'css-loader',
                    'sass-loader'
                ]
            }
        ]
    },
    resolve: {
        extensions: ['.jsx', '.js']
    },
    plugins: [
        new HtmlWebpackPlugin({
            template: path.resolve(__dirname, './src/dictionary.html'),
            filename: 'html/dictionary.html'
        })
    ]
}