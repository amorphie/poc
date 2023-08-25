
export const config = {
  // Step 1: Configure the redis connection
  // @see https://github.com/strapi-community/strapi-plugin-redis
  redis: {
    config: {
      connections: {
        default: {
          connection: {
            host: '127.0.0.1',
            port: 6379,
            db: 0,
          },
          settings: {
            debug: true,
          },
        },
      },
    },
  },
  // Step 2: Configure the redis cache plugin
  "rest-cache": {
    config: {
      provider: {
        name: "redis",
        options: {
          max: 32767,
          connection: "default",
        },
      },
      strategy: {
        enableEtagSupport: true,
        logs: true,
        clearRelatedCache: true,
        maxAge: 3600000,
        contentTypes: [
          // list of Content-Types UID to cache
           "api::course.course",
          // "api::article.article",
          // "api::global.global",
          // "api::homepage.homepage",
          {
            contentType: "api::course.course",
            maxAge: 3600000,
            hitpass: false,
            keys: {
              useQueryParams: false,
              useHeaders: ["accept-encoding"],
            },
            // maxAge: 18000,
            method: "GET",
          }
        ],
      },
    },
  },
};
