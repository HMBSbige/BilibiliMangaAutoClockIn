# Bilibili 漫画自动签到

## Docker

```
docker run -it \
--restart=always \
--name=manga \
hmbsbige/bilibilimangaautoclockin \
$username $password
```

Or

```
docker run -it \
--restart=always \
--name=manga \
hmbsbige/bilibilimangaautoclockin \
$username $password \
$accesstoken $refreshtoken
```