<template>
	<view class="page">
		<!-- 地图容器 -->
		<view class="map-container" :style="{ height: `${mapHeight}px` }">
			<map 
				id="storeMap" 
				class="map" 
				:latitude="currentLocation.latitude" 
				:longitude="currentLocation.longitude"
				:markers="markers" 
				:show-location="true" 
				@markertap="onMarkerTap"
			/>
			<!-- 定位按钮 -->
			<view class="locate-btn" @tap="getUserLocation">
				<u-icon name="map-fill" color="#333" size="20" />
			</view>
		</view>

		<!-- 门店列表容器 -->
		<view class="store-list-container" :style="{ top: `${mapHeight-30}px` }">
			<!-- 搜索栏 -->
			<view class="search-section">
				<view class="search-bar">
					<view class="search-content">
						<!-- 城市选择 -->
						<view class="city-selector" @click="navigateToCity">
							<u-text class="city-text" text="枣庄市"></u-text>
							<u-icon name="arrow-down" size="12" color="#666"></u-icon>
						</view>
			
						<!-- 搜索框 -->
						<view class="search-box">
							<u-search 
								v-model="searchKey" 
								placeholder="搜索门店" 
								shape="round" 
								bgColor="#f8f9fa"
								:showAction="false" 
								@search="handleSearch" 
								@clear="handleSearch" 
							/>
						</view>
					</view>
				</view>
			</view>
			
			<!-- 门店列表 -->
			<scroll-view 
				class="store-list" 
				:style="{ height: `calc(100vh - ${mapHeight}px - 120rpx)` }"
				scroll-y="true"
				@scroll="handleScroll"
				:scroll-top="scrollTop"
			>
				<view 
					class="store-card"
					v-for="store in sortedStores" 
					:key="store.id" 
					@click="selectStore(store)"
				>
					<!-- 门店图片 -->
					<view class="store-image">
						<u-image 
							:src="store.image || defaultStoreImage" 
							width="120rpx" 
							height="120rpx" 
							radius="12rpx"
							mode="aspectFill" 
							lazyLoad 
						/>
					</view>
					
					<!-- 门店信息 -->
					<view class="store-info">
						<!-- 门店名称和距离 -->
						<view class="store-header">
							<u-text class="store-name" :text="store.name"></u-text>
							
							<u-text class="store-distance" :text="store.distance+'km'" align="right"></u-text>
						</view>

						<!-- 制作状态 -->
						<view class="store-status" v-if="store.making">
							<u-tag :text="store.making" size="mini" type="warning" plain />
						</view>

						<!-- 门店地址 -->
						<view class="store-address">{{ store.address }}</view>

						<!-- 营业时间和外卖 -->
						<view class="store-meta">
							<u-text class="business-time" :text="store.time "></u-text>
							<u-text v-if="store.takeaway" class="takeaway-tag" text="可外卖" align="center"></u-text>
						</view>

						<!-- 联系电话 -->
						<view class="store-contact" @tap.stop="makePhoneCall(store.phone)">
							<u-icon name="phone" size="16" color="#ff6b35" />
							<u-text class="phone-text" :text="store.phone"></u-text>
						</view>
					</view>
					
					<!-- 选择按钮 -->
					<view class="select-btn">
						<u-icon name="arrow-right" size="16" color="#ccc" />
					</view>
				</view>

				<!-- 空状态 -->
				<view v-if="!sortedStores.length" class="empty-state">
					<u-empty mode="list" marginTop="120rpx" />
				</view>
			</scroll-view>
		</view>
	</view>
</template>

<script>
export default {
	data() {
		return {
			// 地图相关
			mapHeight: 350, // 地图高度
			minMapHeight: 120, // 最小地图高度
			maxMapHeight: 400, // 最大地图高度
			
			// 滚动相关
			scrollTop: 0,
			lastScrollTop: 0,
			
			// 搜索
			searchKey: '',
			
			// 位置信息
			currentLocation: {
				latitude: 34.7466,
				longitude: 113.7556
			},
			
			// 地图标记
			markers: [],
			
			// 默认图片
			defaultStoreImage: '/static/logo.png',
			
			// 门店数据
			stores: [
				{
					id: 1,
					name: "山东济南历下万象城店",
					address: "万象城二楼2025商铺（承接企业/学校团餐）",
					latitude: 36.651,
					longitude: 117.122,
					distance: 2.34,
					image: "/static/new_logo.png",
					making: "前方3杯制作中",
					time: "09:30-22:00",
					takeaway: true,
					phone: "13864125678"
				},
				{
					id: 2,
					name: "山东淄博张店银座店",
					address: "银座商城一期1F东门（承接会议团餐）",
					latitude: 36.81,
					longitude: 118.05,
					distance: 2.89,
					image: "/static/store_img.png",
					making: "前方4杯制作中",
					time: "10:00-22:00",
					takeaway: true,
					phone: "13953312345"
				},
				{
					id: 3,
					name: "山东烟台芝罘万达店",
					address: "烟台万达广场3F中庭（承接旅游团餐）",
					latitude: 37.54,
					longitude: 121.39,
					distance: 3.78,
					image: "/static/logo_sm.png",
					making: "前方2杯制作中",
					time: "10:00-22:30",
					takeaway: true,
					phone: "13583561234"
				},
				{
					id: 4,
					name: "山东潍坊奎文泰华城店",
					address: "泰华城新天地1F南区（承接展会团餐）",
					latitude: 36.71,
					longitude: 119.12,
					distance: 4.56,
					image: "/static/shop_icon.png",
					making: "前方7杯制作中",
					time: "09:00-21:30",
					takeaway: true,
					phone: "18765678901"
				},
				{
					id: 5,
					name: "山东青岛市南海信广场店",
					address: "海信广场B1层（承接商务/活动团餐）",
					latitude: 36.067,
					longitude: 120.382,
					distance: 5.12,
					image: "/static/new_logo2.png",
					making: "前方6杯制作中",
					time: "10:30-21:00",
					takeaway: true,
					phone: "15805327890"
				},
				{
					id: 6,
					name: "山东临沂兰山和谐广场店",
					address: "和谐广场B1层美食街（承接婚宴团餐）",
					latitude: 35.07,
					longitude: 118.35,
					distance: 6.23,
					image: "/static/brand_logo.png",
					making: "前方8杯制作中",
					time: "09:30-21:45",
					takeaway: true,
					phone: "15053987654"
				},
				{
					id: 7,
					name: "山东济宁任城太白路店",
					address: "太白路万达金街A区（承接培训团餐）",
					latitude: 35.41,
					longitude: 116.59,
					distance: 3.45,
					image: "/static/shop_logo.png",
					making: "前方5杯制作中",
					time: "10:00-21:30",
					takeaway: true,
					phone: "18653712345"
				},
				{
					id: 8,
					name: "山东泰安泰山万达店",
					address: "泰山万达广场2F连廊（承接登山团餐）",
					latitude: 36.19,
					longitude: 117.13,
					distance: 4.78,
					image: "/static/new_store.png",
					making: "前方3杯制作中",
					time: "09:00-22:00",
					takeaway: true,
					phone: "13854897654"
				},
				{
					id: 9,
					name: "山东济宁任城太白路店",
					address: "太白路万达金街A区（承接培训团餐）",
					latitude: 35.41,
					longitude: 116.59,
					distance: 3.45,
					image: "/static/shop_logo.png",
					making: "前方5杯制作中",
					time: "10:00-21:30",
					takeaway: true,
					phone: "18653712345"
				},
				{
					id: 10,
					name: "山东泰安泰山万达店",
					address: "泰山万达广场2F连廊（承接登山团餐）",
					latitude: 36.19,
					longitude: 117.13,
					distance: 4.78,
					image: "/static/new_store.png",
					making: "前方3杯制作中",
					time: "09:00-22:00",
					takeaway: true,
					phone: "13854897654"
				}
			]
		};
	},
	
	computed: {
		sortedStores() {
			let list = [...this.stores];
			if (this.searchKey.trim()) {
				const kw = this.searchKey.trim().toLowerCase();
				list = list.filter(s => s.name.toLowerCase().includes(kw));
			}
			return list.sort((a, b) => a.distance - b.distance);
		}
	},
	
	onLoad() {
		this.initMarkers();
		this.getUserLocation();
	},
	
	methods: {
		// 处理滚动事件
		handleScroll(e) {
			const scrollTop = e.detail.scrollTop;
			const deltaY = scrollTop - this.lastScrollTop;
			
			// 向上滚动时缩小地图
			if (deltaY > 0 && this.mapHeight > this.minMapHeight) {
				this.mapHeight = Math.max(this.minMapHeight, this.mapHeight - deltaY * 0.5);
			}
			// 向下滚动时放大地图
			else if (deltaY < 0 && this.mapHeight < this.maxMapHeight) {
				this.mapHeight = Math.min(this.maxMapHeight, this.mapHeight - deltaY * 0.5);
			}
			
			this.lastScrollTop = scrollTop;
		},
		
		// 选择门店
		selectStore(store) {
			uni.showToast({
				title: `已选择：${store.name}`,
				icon: 'success'
			});
			// 这里可以跳转到点单页面
			// uni.navigateTo({
			//     url: `/pages/order/order?storeId=${store.id}`
			// });
		},
		
		// 初始化地图标记
		initMarkers() {
			this.markers = this.stores.map(s => ({
				id: s.id,
				latitude: s.latitude,
				longitude: s.longitude,
				iconPath: '/static/marker.png',
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
			}));
		},
		
		// 获取用户位置
		getUserLocation() {
			uni.getLocation({
				type: 'gcj02',
				success: res => {
					this.currentLocation = {
						latitude: res.latitude,
						longitude: res.longitude
					};
					this.calculateDistances();
				},
				fail: () => {
					uni.showToast({
						title: '定位失败',
						icon: 'none'
					});
				}
			});
		},
		
		// 计算距离
		calculateDistances() {
			this.stores.forEach(s => {
				s.distance = this.getDistance(
					this.currentLocation.latitude,
					this.currentLocation.longitude,
					s.latitude,
					s.longitude
				);
			});
		},
		
		// 计算两点间距离
		getDistance(lat1, lng1, lat2, lng2) {
			const rad = d => (d * Math.PI) / 180;
			const R = 6378.137;
			const dLat = rad(lat2 - lat1);
			const dLng = rad(lng2 - lng1);
			const a = Math.sin(dLat / 2) ** 2 +
				Math.cos(rad(lat1)) * Math.cos(rad(lat2)) * Math.sin(dLng / 2) ** 2;
			return +(2 * R * Math.asin(Math.sqrt(a))).toFixed(1);
		},
		
		// 标记点击事件
		onMarkerTap(e) {
			const store = this.stores.find(s => s.id === e.markerId);
			store && this.selectStore(store);
		},
		
		// 搜索处理
		handleSearch() {
			// 搜索逻辑已在 computed 中处理
		},
		
		// 拨打电话
		makePhoneCall(phone) {
			uni.makePhoneCall({
				phoneNumber: phone
			});
		},
		
		// 跳转到城市选择
		navigateToCity() {
			uni.navigateTo({
				url: '/pages/map/choosecity'
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

	/* 地图容器样式 */
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
	}

	/* 门店列表容器样式 */
	.store-list-container {
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

	/* 搜索区域样式 */
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

	/* 门店列表样式 */
	.store-list {
		padding: 0 30rpx;
		background-color: #fff;
	}

	/* 门店卡片样式 */
	.store-card {
		display: flex;
		align-items: flex-start;
		padding: 30rpx 0;
		border-bottom: 1rpx solid #f5f5f5;
		transition: all 0.3s ease;
		
		&:last-child {
			border-bottom: none;
		}
		
		&:active {
			background-color: #f8f9fa;
			transform: translateX(4rpx);
		}

		.store-image {
			margin-right: 24rpx;
			flex-shrink: 0;
		}

		.store-info {
			flex: 1;
			min-width: 0;

			.store-header {
				display: flex;
				justify-content: space-between;
				align-items: flex-start;
				margin-bottom: 12rpx;

				.store-name {
					font-size: 32rpx;
					font-weight: 600;
					color: #333;
					line-height: 1.4;
					flex: 1;
					margin-right: 16rpx;
				}

				.store-distance {
					font-size: 26rpx;
					color: #ff6b35;
					font-weight: 500;
					flex-shrink: 0;
				}
			}

			.store-status {
				margin-bottom: 12rpx;
			}

			.store-address {
				font-size: 26rpx;
				color: #666;
				line-height: 1.5;
				margin-bottom: 12rpx;
			}

			.store-meta {
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

			.store-contact {
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

	/* 空状态样式 */
	.empty-state {
		padding: 60rpx 0;
	}
</style>