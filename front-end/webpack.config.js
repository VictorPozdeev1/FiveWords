const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');

const prodMode = process.env.NODE_ENV === 'production';

module.exports = {
    entry: {
        dictionary: path.resolve(__dirname, './src/dictionary-index.js')
    },
    output: {
        path: path.resolve(__dirname, './dist/script'),
        filename: '[name].[contenthash].js'
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
                use: ['style-loader', 'css-loader', 'sass-loader']
            }
        ]
    },
    resolve: {
        extensions: ['.jsx', '.js']
    },
    plugins: [
        new HtmlWebpackPlugin({
            template: path.resolve(__dirname, './src/dictionary2.html'),
            filename: 'dictionary.html'
        })
    ]
}