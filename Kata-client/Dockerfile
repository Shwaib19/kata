# Nuxt builds a server bundle, so the image runs Node rather than serving static files.
FROM oven/bun:1-alpine AS build
WORKDIR /app

# Everything is copied before installing because the postinstall step runs `nuxt prepare`,
# which needs the project, not only its manifest.
COPY . ./

RUN bun install --frozen-lockfile
RUN bun run build

# Only the build output is shipped: no sources, no node_modules, no Bun.
FROM node:24-alpine
WORKDIR /app

COPY --from=build /app/.output ./.output

ENV NITRO_PORT=3000
ENV NITRO_HOST=0.0.0.0
EXPOSE 3000

CMD ["node", ".output/server/index.mjs"]
