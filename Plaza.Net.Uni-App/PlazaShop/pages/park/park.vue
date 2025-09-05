<template>
  <view class="parking-page">
    <!-- 广场信息 -->
    <view class="plaza-card">
      <view class="plaza-title">
        <u-text :text="currentPlaza.name" align="center" class="plaza-name" />
        <u-text :text="currentPlaza.address" align="center" class="plaza-address" />
      </view>

      <!-- 车牌输入 -->
      <view class="lp-input">
        <u-text text="输入车牌号缴费" class="lp-input-title" />
        <view class="lp-plate" @click="openCarKeyboard">
          <u-code-input
            v-model="plateStr"
            :maxlength="8"
            size="40"
            space="6rpx"
            mode="box"
            font-size="32rpx"
            color="#333"
            inactive-color="#e5e5e5"
            active-color="#5555ff"
            hairline bold disabled
          />
        </view>
        <view class="lp-btn">
          <u-button type="primary" size="normal" shape="circle" :loading="queryLoading" @click="queryByPlate">
            查询
          </u-button>
        </view>
      </view>

      <!-- 车位统计 -->
      <view class="lp-stats">
        <view class="stat-item">
          <u-text :text="totalSpots" class="stat-value" align="center"/>
          <u-text text="总车位" class="stat-label" />
        </view>
        <view class="stat-item">
          <u-text :text="freeSpots" color="#00b42a" class="stat-value" align="center"/>
          <u-text text="空闲" class="stat-label" />
        </view>
        <view class="stat-item">
          <u-text :text="occupiedSpots" color="#f53f3f" class="stat-value" align="center"/>
          <u-text text="占用" class="stat-label" />
        </view>
        <view class="stat-item">
          <u-text :text="maintenanceSpots" color="#86909c" class="stat-value" align="center"/>
          <u-text text="维修中" class="stat-label" />
        </view>
      </view>
    </view>

    <!-- 楼层筛选器 -->
    <view class="floor-filter">
      <u-scroll-list scroll-x class="floor-scroll" :show-scrollbar="false">
        <view v-for="floor in floors" :key="floor.id" class="floor-item"
              :class="{ active: currentFloorId === floor.id }" @click="switchFloor(floor.id)">
          {{ floor.name }}
        </view>
      </u-scroll-list>
    </view>

    <!-- 停车场区域列表 -->
    <view class="parking-areas">
      <view class="section-title"><u-text text="停车场区域" /></view>
      <u-skeleton v-if="loading" :rows="3" rows-width="['100%','100%','100%']" rows-height="[200]" rows-radius="10"
                  margin="0 30rpx 20rpx" />
      <u-empty v-else-if="currentFloorAreas.length === 0" mode="list" icon="map"
               text="当前楼层暂无停车场区域" />
      <view v-else class="area-list">
        <view v-for="area in currentFloorAreas" :key="area.id" class="area-card"
              @click="gotoAreaDetail(area)">
          <view class="area-header">
            <view class="area-name">
              <u-icon name="car" color="#5555ff" size="20" />
              <u-text :text="area.name" margin="0 0 0 10rpx" />
            </view>
            <u-tag
              :text="area.freeRate > 0 ? area.freeRate + '% 空闲' : '已满'"
              :bg-color="area.freeRate > 50 ? '#e8f6e8' : area.freeRate > 0 ? '#fff8e6' : '#ffe8e8'"
              :color="area.freeRate > 50 ? '#00b42a' : area.freeRate > 0 ? '#ff8f1f' : '#f53f3f'"
              size="mini" plain
            />
          </view>
          <view class="area-body">
            <view class="spot-stats">
              <view class="spot-item"><view class="spot-dot free"></view><u-text :text="'空闲: ' + area.freeSpots" /></view>
              <view class="spot-item"><view class="spot-dot occupied"></view><u-text :text="'占用: ' + area.occupiedSpots" /></view>
              <view class="spot-item"><view class="spot-dot maintenance"></view><u-text :text="'维修: ' + area.maintenanceSpots" /></view>
            </view>
          </view>
        </view>
      </view>
    </view>

    <!-- 查询后停车记录 -->
    <view class="my-parking" v-if="queriedRecord">
      <view class="section-title"><u-text text="当前停车记录" /></view>
      <view class="parking-record-card">
        <u-cell-group :border="false">
          <u-cell title="车牌号" :value="queriedRecord.plateNumber" :border="false" />
          <u-cell title="入场时间" :value="formatTime(queriedRecord.entryTime)" :border="false" />
          <u-cell title="停车时长" :value="calculateParkingTime(queriedRecord.entryTime)" :border="false" />
          <u-cell title="预计费用" :value="'¥' + queriedRecord.parkingFee.toFixed(2)"
                  :border="false" valueStyle="color:#f53f3f"/>
        </u-cell-group>
        <u-button type="primary" size="medium" shape="circle" class="pay-button"
                  @click="gotoPayment">缴纳停车费</u-button>
      </view>
    </view>

    <!-- 官方车牌键盘 -->
    <u-keyboard :show="carKeyboardShow" mode="car" overlay
                @change="onKeyChange" @backspace="onBackspace"
                @confirm="onCarInputConfirm" @cancel="carKeyboardShow = false" />
  </view>
</template>

<script>
export default {
  data() {
    return {
      loading: true,
      currentPlaza: { id: null, name: '', address: '' },
      floors: [],
      currentFloorId: null,
      allAreas: [],
      totalSpots: 0,
      freeSpots: 0,
      occupiedSpots: 0,
      maintenanceSpots: 0,
      plateStr: '',
      queryLoading: false,
      queriedRecord: null,
      carKeyboardShow: false
    };
  },
  computed: {
    currentFloorAreas() {
      return this.allAreas.filter(a => a.floorId === this.currentFloorId);
    }
  },
  onLoad(options) {
    const plazaId = options?.plazaId || uni.getStorageSync('selectedPlaza')?.id;
    if (!plazaId) {
      uni.navigateTo({ url: '/pages/map/mapTest?from=park' });
      return;
    }
    this.loadPlazaDetail(plazaId);
  },
  onShow() {
    uni.$once('plazaSelectedFromMap', (plaza) => {
      this.loadPlazaDetail(plaza.id);
    });
  },
  methods: {
    async loadPlazaDetail(plazaId) {
      uni.showLoading({ title: '加载中' });
      const res = await uni.request({ url: `/api/park/${plazaId}/floors`, method: 'GET' });
      uni.hideLoading();
      if (res.statusCode === 200 && res.data.length) {
        this.floors = res.data;
        this.currentFloorId = this.floors[0].id;
        this.loadAreas(this.currentFloorId);
      } else {
        this.floors = [];
        this.allAreas = [];
      }
    },
    async loadAreas(floorId) {
      this.loading = true;
      const res = await uni.request({ url: `/api/park/${floorId}/parking-summary`, method: 'GET' });
      this.loading = false;
      if (res.statusCode === 200 && res.data) {
        this.allAreas = res.data.map(area => ({
          id: area.areaId,
          name: area.areaName,
          floorId,
          freeSpots: area.statusCounts.find(s => s.label === '空闲')?.count || 0,
          occupiedSpots: area.statusCounts.find(s => s.label === '占用')?.count || 0,
          maintenanceSpots: area.statusCounts.find(s => s.label === '维护')?.count || 0,
          freeRate: area.totalSpots > 0
            ? Math.round((area.statusCounts.find(s => s.label === '空闲')?.count || 0) / area.totalSpots * 100)
            : 0
        }));
        this.calculateSpotStats();
      } else {
        this.allAreas = [];
      }
    },
    switchFloor(id) {
      this.currentFloorId = id;
      this.loadAreas(id);
    },
    calculateSpotStats() {
      const areas = this.currentFloorAreas;
      this.totalSpots = areas.reduce((s, a) => s + a.freeSpots + a.occupiedSpots + a.maintenanceSpots, 0);
      this.freeSpots = areas.reduce((s, a) => s + a.freeSpots, 0);
      this.occupiedSpots = areas.reduce((s, a) => s + a.occupiedSpots, 0);
      this.maintenanceSpots = areas.reduce((s, a) => s + a.maintenanceSpots, 0);
    },
    openCarKeyboard() { this.carKeyboardShow = true; },
    onKeyChange(val) { if (this.plateStr.length < 8) this.plateStr += val; },
    onBackspace() { this.plateStr = this.plateStr.slice(0, -1); },
    onCarInputConfirm() { this.carKeyboardShow = false; },
    queryByPlate() {
      if (!this.plateStr.trim()) { uni.showToast({ title: '请输入车牌号', icon: 'none' }); return; }
      this.queriedRecord = { plateNumber: this.plateStr, entryTime: '2023-08-09T08:00:00', parkingFee: 8 };
    },
    gotoPayment() { uni.showToast({ title: '跳转到支付页', icon: 'none' }); },
    formatTime(timeStr) {
      const d = new Date(timeStr);
      return `${d.getMonth() + 1}月${d.getDate()}日 ${d.getHours().toString().padStart(2, '0')}:${d.getMinutes().toString().padStart(2, '0')}`;
    },
    calculateParkingTime(entryTimeStr) {
      const diff = Math.floor((Date.now() - new Date(entryTimeStr).getTime()) / 1000);
      const h = Math.floor(diff / 3600);
      const m = Math.floor((diff % 3600) / 60);
      return h > 0 ? `${h}小时${m}分钟` : `${m}分钟`;
    }
  }
};
</script>
<style lang="scss" scoped>
.parking-page {
  min-height: 100vh;
  background: #f5f7fb;
  padding-bottom: 60rpx;
}

.plaza-card {
  margin: 30rpx;
  padding: 40rpx 30rpx;
  background: #ffffff;
  border-radius: 24rpx;
  box-shadow: 0 8rpx 24rpx rgba(0, 0, 0, 0.06);
  position: relative;
  overflow: hidden;
}
.plaza-card::before {
  content: "";
  position: absolute;
  top: -100rpx;
  left: -100rpx;
  width: 200rpx;
  height: 200rpx;
  background: linear-gradient(135deg, rgba(84, 104, 255, 0.1), transparent 70%);
  border-radius: 50%;
}
.plaza-title {
  text-align: center;
  margin-bottom: 36rpx;
}
.plaza-name {
  font-size: 36rpx;
  font-weight: 700;
  color: #1c1e21;
}
.plaza-address {
  font-size: 26rpx;
  color: #8a94a6;
  margin-top: 8rpx;
}

.lp-input {
  display: flex;
  flex-direction: column;
  align-items: center;
  margin-bottom: 36rpx;
}
.lp-input-title {
  font-size: 28rpx;
  font-weight: 500;
  color: #333;
  margin-bottom: 24rpx;
}
.lp-plate {
  margin-bottom: 26rpx;
}

.lp-stats {
  display: flex;
  justify-content: space-around;
}
.stat-item {
  display: flex;
  flex-direction: column;
  align-items: center;
}
.stat-value {
  font-size: 34rpx;
  font-weight: 700;
  color: #1c1e21;
}
.stat-label {
  font-size: 24rpx;
  color: #8a94a6;
  margin-top: 6rpx;
}

.floor-filter {
  margin: 0 30rpx 30rpx;
}
.floor-scroll {
  display: flex;
  padding-bottom: 8rpx;
}
.floor-item {
  flex-shrink: 0;
  padding: 14rpx 32rpx;
  margin-right: 20rpx;
  font-size: 28rpx;
  color: #8a94a6;
  background: #ffffff;
  border-radius: 40rpx;
  box-shadow: 0 4rpx 12rpx rgba(0, 0, 0, 0.04);
  transition: all 0.3s ease;
}
.floor-item.active {
  background: #5468ff;
  color: #fff;
  font-weight: 600;
  box-shadow: 0 6rpx 16rpx rgba(84, 104, 255, 0.3);
}

.parking-areas {
  padding: 0 30rpx;
}
.section-title {
  font-size: 30rpx;
  font-weight: 600;
  color: #1c1e21;
  margin: 20rpx 0 30rpx;
  position: relative;
  padding-left: 20rpx;
}
.section-title::before {
  content: "";
  position: absolute;
  left: 0;
  top: 50%;
  transform: translateY(-50%);
  width: 6rpx;
  height: 28rpx;
  background: #5468ff;
  border-radius: 3rpx;
}
.area-card {
  background: #ffffff;
  border-radius: 24rpx;
  padding: 32rpx 30rpx;
  margin-bottom: 26rpx;
  box-shadow: 0 8rpx 24rpx rgba(0, 0, 0, 0.06);
  transition: transform 0.25s, box-shadow 0.25s;
}
.area-card:active {
  transform: translateY(-4rpx);
  box-shadow: 0 12rpx 32rpx rgba(0, 0, 0, 0.08);
}
.area-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 22rpx;
}
.area-name {
  font-size: 30rpx;
  font-weight: 600;
  color: #1c1e21;
  display: flex;
  align-items: center;
}
.spot-stats {
  display: flex;
  gap: 28rpx;
}
.spot-item {
  display: flex;
  align-items: center;
  font-size: 26rpx;
  color: #8a94a6;
}
.spot-dot {
  width: 20rpx;
  height: 10rpx;
  border-radius: 5rpx;
  margin-right: 8rpx;
}
.spot-dot.free {
  background: #00c389;
}
.spot-dot.occupied {
  background: #ff4757;
}
.spot-dot.maintenance {
  background: #b0b9c6;
}

.my-parking {
  padding: 0 30rpx;
}
.parking-record-card {
  background: #ffffff;
  border-radius: 24rpx;
  padding: 30rpx;
  box-shadow: 0 8rpx 24rpx rgba(0, 0, 0, 0.06);
}
.pay-button {
  margin-top: 30rpx;
  width: 100%;
  height: 80rpx;
  font-size: 30rpx;
  font-weight: 600;
  border-radius: 40rpx;
  background: linear-gradient(135deg, #5468ff, #7b8bff);
  box-shadow: 0 6rpx 20rpx rgba(84, 104, 255, 0.3);
}
</style>