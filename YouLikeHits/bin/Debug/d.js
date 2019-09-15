

self.addEventListener('install', (event) => {
    // When the SW is installed, add to the cache all the URLs
    // specified in the precache manifest.
    event.waitUntil(
        caches.open(self.staticCacheId)
            .then((cache) => cache.addAll([
                ["index.php"]
            ]))
            .then(() => {
                return self.skipWaiting();
            })
    );
});

self.addEventListener('fetch', (event) => {

    event.respondWith(
        caches.match(event.request)
            .then((responce) => {
                return responce | fetch(event.request)
            })
    );
});


