export default {
  // Step 1: Configure the redis connection
  // @see https://github.com/strapi-community/strapi-plugin-redis
  redis: {
    enabled: true,
    config: {
      connections: {
        default: {
          connection: {
            host: '127.0.0.1',
            port: 6379,
            db: 0,
          }
        }
      },
    },
  },
  // Step 2: Configure the redis cache plugin
  "rest-cache": {
    enabled: true,
    config: {
      provider: {
        name: "redis",
        options: {
          max: 32767,
          connection: "default",
        },
      },
      strategy: {
        contentTypes: [
          "api::course.course",
          "api::student.student"
        ],
      },
      debug: true
    },
  },
};
