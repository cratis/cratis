// Copyright (c) Aksio Insurtech. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import { defineConfig } from 'vite';
import react from "@vitejs/plugin-react";
import path from 'path';

export default defineConfig({
    build: {
        outDir: './wwwroot',
        assetsDir: '',
        rollupOptions: {
            external: [
                './backup',
            ],
        },
    },
    plugins: [
        react()
    ],
    server: {
        port: 9000,
        open: true,
        proxy: {
            '/api': {
                target: 'http://localhost:8080',
                ws: true
            },
            '/swagger': {
                target: 'http://localhost:8080'
            }
        }
    },
    resolve: {
        alias: {
            'API': path.resolve('./API'),
            'assets': path.resolve('./src/assets'),
            'Components': path.resolve('./src/Components'),
            'Routing': path.resolve('./src/Routing'),
            'Styles': path.resolve('./src/Styles'),
        }
    }

});
