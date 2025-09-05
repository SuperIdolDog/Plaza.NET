<template>
  <view class="page">
    <!-- 地图容器 -->
    <view class="map-container" :style="{ height: `${mapHeight}px` }">
      <map
        id="plazaMap"
        class="map"
        :latitude="currentLocation.latitude"
        :longitude="currentLocation.longitude"
        :markers="markers"
        :show-location="true"
        :enable-zoom="true"
        :enable-scroll="true"
        :enable-rotate="false"
        :scale="scale"
        :min-scale="3"
        :max-scale="20"
      />
      <view class="locate-btn" @tap="getUserLocation">
        <u-icon name="map-fill" color="#333" size="20" />
      </view>
      <view class="zoom-controls">
        <view class="zoom-btn" @tap="zoomOut"><u-icon name="plus" color="#333" size="20" /></view>
        <view class="zoom-btn" @tap="zoomIn"><u-icon name="minus" color="#333" size="20" /></view>
      </view>
    </view>

    <!-- 广场列表 -->
    <view class="plaza-list-container" :style="{ top: `${mapHeight - 30}px` }">
      <view class="search-section">
        <view class="search-bar">
          <view class="search-content">
            <view class="city-selector" @click="navigateToCity">
              <u-text class="city-text" :text="selectedCity" />
              <u-icon name="arrow-down" size="12" color="#666" />
            </view>
            <u-search
              v-model="searchKey"
              placeholder="搜索广场"
              shape="round"
              bgColor="#f8f9fa"
              :showAction="false"
              @search="handleSearch"
              @clear="handleSearch"
            />
          </view>
        </view>
      </view>

      <scroll-view
        class="plaza-list"
        :style="{ height: `calc(100vh - ${mapHeight}px - 120rpx)` }"
        scroll-y
        @scroll="handleScroll"
        :scroll-top="scrollTop"
      >
        <view class="list-center">
          <view
            v-for="plaza in sortedplazas"
            :key="plaza.id"
            class="plaza-card"
            @click="selectplaza(plaza)"
          >
            <view class="plaza-info">
              <view class="plaza-header">
                <u-text class="plaza-name" :text="plaza.name" />
                <u-text class="plaza-distance" :text="plaza.distance + 'km'" align="right" />
              </view>
              <view class="plaza-address">{{ plaza.address }}</view>
            </view>
            <u-icon name="arrow-right" size="16" color="#ccc" />
          </view>

          <view v-if="!sortedplazas.length" class="empty-state">
            <u-empty mode="list" marginTop="120rpx" />
          </view>
        </view>
      </scroll-view>
    </view>
  </view>
</template>

<script>
export default {
  data() {
    return {
      selectedCity: '正在定位中...',
      hasUserSelectedCity: false,
      mapHeight: 350,
      minMapHeight: 120,
      maxMapHeight: 400,
      scale: 16,
      scrollTop: 0,
      lastScrollTop: 0,
      searchKey: '',
      currentLocation: { latitude: 39.904956, longitude: 116.389449 },
      plazas: [],
      markers: [],
      myLocationMarker: null
    };
  },
  computed: {
    sortedplazas() {
      let list = [...this.plazas];
      if (this.searchKey.trim()) {
        const kw = this.searchKey.trim().toLowerCase();
        list = list.filter((s) => s.name.toLowerCase().includes(kw));
      }
      return list.sort((a, b) => a.distance - b.distance);
    }
  },
  onLoad(options) {
    if (options && options.city) {
      this.selectedCity = decodeURIComponent(options.city);
      this.hasUserSelectedCity = true;
    }
    this.getUserLocation();
  },
  onShow() {
    uni.$once('citySelected', (city) => {
      this.selectedCity = city;
      this.hasUserSelectedCity = true;
      this.loadplazas();
    });
  },
  methods: {
    async loadplazas() {
      if (!this.currentLocation.latitude) return;
      uni.showLoading({ title: '加载中' });
      try {
        const res = await uni.request({
          url: '/api/plaza/nearby',
          method: 'GET',
          data: {
            lat: this.currentLocation.latitude,
            lng: this.currentLocation.longitude,
            city: this.selectedCity,
            page: 1,
            size: 50
          }
        });
        if (res.data.success) {
          this.plazas = res.data.data || [];
          this.initMarkers();
        } else {
          uni.showToast({ title: '获取门店数据失败', icon: 'none' });
        }
      } catch {
        uni.showToast({ title: '获取门店失败', icon: 'none' });
      } finally {
        uni.hideLoading();
      }
    },
    async reverseCity(lat, lng) {
      try {
        const res = await uni.request({
          url: '/api/plaza/city',
          method: 'GET',
          data: { lat, lng }
        });
        if (res.data.status === 0 && res.data.result) {
          let city = res.data.result.address_component.city;
          const province = res.data.result.address_component.province;
          if (city.endsWith('市辖区') && province) return province;
          return city;
        }
        return '未知城市';
      } catch {
        return '未知城市';
      }
    },
    zoomIn() {
      if (this.scale < 20) this.scale += 1;
    },
    zoomOut() {
      if (this.scale > 3) this.scale -= 1;
    },
    handleScroll(e) {
      const scrollTop = e.detail.scrollTop;
      const deltaY = scrollTop - this.lastScrollTop;
      if (deltaY > 0 && this.mapHeight > this.minMapHeight) {
        this.mapHeight = Math.max(this.minMapHeight, this.mapHeight - deltaY * 0.5);
      } else if (deltaY < 0 && this.mapHeight < this.maxMapHeight) {
        this.mapHeight = Math.min(this.maxMapHeight, this.mapHeight - deltaY * 0.5);
      }
      this.lastScrollTop = scrollTop;
    },
    selectplaza(plaza) {
      uni.setStorageSync('selectedPlaza', {
        id: plaza.id,
        name: plaza.name,
        city: plaza.city
      });
	   uni.$emit('plazaSelected', plaza);
      uni.navigateBack();
    },
    initMarkers() {
      this.markers = [];
      if (this.myLocationMarker) this.markers.push(this.myLocationMarker);
      this.plazas.forEach((s) =>
        this.markers.push({
          id: s.id,
          latitude: s.latitude,
          longitude: s.longitude,
          iconPath: '/static/location.png',
          width: 30,
          height: 30,
          callout: {
            content: s.name,
            color: '#333',
            fontSize: 12,
            borderRadius: 4,
            bgColor: '#ffffff',
            padding: 5,
            display: 'ALWAYS'
          }
        })
      );
    },
    getUserLocation() {
      // #ifdef H5
      if (this.hasUserSelectedCity) {
        this._getLocationOnly();
        return;
      }
      if (!navigator.geolocation) {
        uni.showToast({ title: '浏览器不支持定位', icon: 'none' });
        this.useBeijingFallback();
        return;
      }
      navigator.geolocation.getCurrentPosition(
        async (pos) => {
          const { latitude, longitude } = pos.coords;
          uni.showToast({ title: '定位成功', icon: 'success' });
          this.currentLocation = { latitude, longitude };
          this.selectedCity = await this.reverseCity(latitude, longitude);
          this.hasUserSelectedCity = false;
          this.loadplazas();
          this.setMyLocationMarker(latitude, longitude);
          this.initMarkers();
        },
        () => {
          uni.showToast({ title: '定位失败，使用北京', icon: 'none' });
          this.useBeijingFallback();
        },
        { enableHighAccuracy: true, timeout: 10000 }
      );
      // #endif

      // #ifndef H5
      uni.getLocation({
        type: 'gcj02',
        success: async (res) => {
          uni.showToast({ title: '定位成功', icon: 'success' });
          this.currentLocation = { latitude: res.latitude, longitude: res.longitude };
          this.selectedCity = await this.reverseCity(res.latitude, res.longitude);
          this.hasUserSelectedCity = false;
          this.loadplazas();
          this.setMyLocationMarker(res.latitude, res.longitude);
        },
        fail: () => {
          uni.showToast({ title: '定位失败，使用北京', icon: 'none' });
          this.useBeijingFallback();
        }
      });
      // #endif
    },
    _getLocationOnly() {
      return new Promise((resolve) => {
        // #ifdef H5
        if (!navigator.geolocation) {
          resolve();
          return;
        }
        navigator.geolocation.getCurrentPosition(
          (pos) => {
            const { latitude, longitude } = pos.coords;
            this.currentLocation = { latitude, longitude };
            this.setMyLocationMarker(latitude, longitude);
            this.loadplazas();
            resolve();
          },
          () => resolve()
        );
        // #endif

        // #ifndef H5
        uni.getLocation({
          type: 'gcj02',
          success: (res) => {
            this.currentLocation = { latitude: res.latitude, longitude: res.longitude };
            this.setMyLocationMarker(res.latitude, res.longitude);
            this.loadplazas();
            resolve();
          },
          fail: resolve
        });
        // #endif
      });
    },
    setMyLocationMarker(lat, lng) {
      this.myLocationMarker = {
        id: 999,
        latitude: lat,
        longitude: lng,
        iconPath: '/static/my-location.png',
        width: 36,
        height: 36,
        anchor: { x: 0.5, y: 0.5 }
      };
      this.initMarkers();
    },
    useBeijingFallback() {
      this.currentLocation = { latitude: 39.904956, longitude: 116.389449 };
      this.selectedCity = '北京市';
      this.hasUserSelectedCity = false;
      this.loadplazas();
    },
    handleSearch() {},
    navigateToCity() {
      uni.navigateTo({
        url: `/pages/map/choosecity?city=${encodeURIComponent(this.selectedCity)}`
      });
    }
  }
};
</script>
<style lang="scss" scoped>
.page {
  height: 100vh;
  background-color: #f8f9fa;
  position: relative;
}

.map-container {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 10;
  background-color: #fff;
  transition: height 0.3s ease-out;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);

  .map {
    width: 100%;
    height: 100%;
  }

  .locate-btn {
    position: absolute;
    right: 30rpx;
    bottom: 80rpx;
    width: 80rpx;
    height: 80rpx;
    background: #fff;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.15);
    transition: all 0.3s ease;

    &:active {
      transform: scale(0.95);
    }
  }

  .zoom-controls {
    position: absolute;
    right: 30rpx;
    bottom: 180rpx;
    display: flex;
    flex-direction: column;
    gap: 20rpx;
    z-index: 100;

    .zoom-btn {
      width: 80rpx;
      height: 80rpx;
      background: #fff;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 36rpx;
      font-weight: bold;
      color: #333;
      box-shadow: 0 4rpx 16rpx rgba(0, 0, 0, 0.15);
    }
  }
}

.plaza-list-container {
  position: fixed;
  left: 0;
  right: 0;
  bottom: 0;
  z-index: 20;
  background-color: #fff;
  border-radius: 24rpx 24rpx 0 0;
  box-shadow: 0 -4rpx 20rpx rgba(0, 0, 0, 0.08);
  transition: top 0.3s ease-out;
}

.search-section {
  position: sticky;
  top: 0;
  z-index: 30;
  background-color: #fff;
  border-radius: 24rpx 24rpx 0 0;
  padding: 20rpx 30rpx 30rpx;
  border-bottom: 1rpx solid #f0f0f0;
}

.search-bar {
  .search-content {
    display: flex;
    align-items: center;
    gap: 20rpx;
  }

  .city-selector {
    display: flex;
    align-items: center;
    gap: 8rpx;
    padding: 16rpx 20rpx;
    background-color: #f8f9fa;
    border-radius: 20rpx;
    border: 1rpx solid #e9ecef;

    .city-text {
      font-size: 28rpx;
      color: #333;
      font-weight: 500;
    }
  }

  .search-box {
    flex: 1;
  }
}

.plaza-list {
  padding: 0 30rpx;
  background-color: #fff;
  box-sizing: border-box;
}
.list-center {
  display: flex;
  flex-direction: column;
  align-items: center;   /* 水平居中 */
  padding: 20rpx 0;      /* 上下留白 */
}
.plaza-card {
  width: 92%;
   max-width: 700rpx;
   margin: 0 auto 20rpx auto; // 👈 关键修复
   padding: 30rpx 24rpx;
   background: #fff;
   border-radius: 20rpx;
   box-shadow: 0 8rpx 20rpx rgba(0, 0, 0, 0.06);
   display: flex;
   align-items: center;
   transition: transform 0.2s ease;
  &:last-child {
    border-bottom: none;
  }

  &:active {
    background-color: #f8f9fa;
    transform: translateX(4rpx);
  }

  .plaza-image {
    margin-right: 24rpx;
    flex-shrink: 0;
  }

  .plaza-info {
    flex: 1;
    min-width: 0;

    .plaza-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 12rpx;

      .plaza-name {
        font-size: 32rpx;
        font-weight: 600;
        color: #333;
        line-height: 1.4;
        flex: 1;
        margin-right: 16rpx;
      }

      .plaza-distance {
        font-size: 26rpx;
        color: #ff6b35;
        font-weight: 500;
        flex-shrink: 0;
      }
    }

    .plaza-status {
      margin-bottom: 12rpx;
    }

    .plaza-address {
      font-size: 26rpx;
      color: #666;
      line-height: 1.5;
      margin-bottom: 12rpx;
    }

    .plaza-meta {
      display: flex;
      align-items: center;
      gap: 16rpx;
      margin-bottom: 12rpx;

      .business-time {
        font-size: 24rpx;
        color: #999;
      }

      .takeaway-tag {
        font-size: 22rpx;
        color: #1890ff;
        background-color: #e6f7ff;
        padding: 4rpx 12rpx;
        border-radius: 12rpx;
        border: 1rpx solid #91d5ff;
      }
    }

    .plaza-contact {
      display: flex;
      align-items: center;
      gap: 8rpx;

      .phone-text {
        font-size: 26rpx;
        color: #ff6b35;
      }
    }
  }

  .select-btn {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 60rpx;
    height: 60rpx;
    margin-left: 16rpx;
    flex-shrink: 0;
  }
}

.empty-state {
  padding: 60rpx 0;
}
</style>