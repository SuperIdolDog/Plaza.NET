<template>
  <view class="profile-container">
    <!-- 商户信息卡片 -->
    <view class="merchant-card">
      <!-- 轮播图 -->
      <u-swiper :list="merchantInfo.swiperImages" height="300" 
                :indicator="true" mode="line" :autoplay="true"
                interval="3000" radius="0"></u-swiper>

      <!-- 商户头像和基本信息 -->
      <view class="merchant-header">
        <view class="logo-wrapper">
          <u-avatar :src="merchantInfo.logo" size="90" :fade="true"></u-avatar>
        </view>

        <view class="base-info">
          <u-text class="merchant-name" :text="merchantInfo.name" 
                  bold align="center" size="18"></u-text>

          <!-- 标签滚动 -->
          <u-scroll-list :indicator="false" v-if="merchantInfo.tags && merchantInfo.tags.length">
            <view class="tags-container">
              <u-tag v-for="(tag, index) in merchantInfo.tags" :key="index" 
                     :text="tag" size="mini" type="error" shape="circle" plain
                     class="tag-item" :customStyle="{marginRight: '8rpx'}"></u-tag>
            </view>
          </u-scroll-list>

          <u-text class="description" :text="merchantInfo.description || '暂无商户简介'" 
                  size="13" color="#666" align="center"></u-text>
        </view>

        <view class="action-buttons">
          <u-button shape="circle" size="mini" text="联系商家" type="error" 
                    @click="contactMerchant"
                    :customStyle="{height: '30px', padding: '0 12px'}"></u-button>
        </view>
      </view>

      <!-- 商户数据统计 -->
      <u-grid :col="3" :border="false">
        <u-grid-item v-for="(item, index) in statsData" :key="index">
          <u-text class="stat-num" :text="item.value" bold size="16"></u-text>
          <u-text class="stat-label" :text="item.label" size="12" color="#999"></u-text>
        </u-grid-item>
      </u-grid>

      <!-- 商户地址和营业时间 -->
      <u-cell-group v-if="merchantInfo.address || merchantInfo.businessHours">
        <u-cell v-if="merchantInfo.address" :title="merchantInfo.address" 
                @click="copyAddress" :label="merchantInfo.address">
          <template #icon>
            <u-icon name="map" size="20" color="#FF2442"></u-icon>
          </template>
          <template #right-icon>
            <u-icon name="arrow-right" size="16" color="#ccc"></u-icon>
          </template>
        </u-cell>
        <u-cell v-if="merchantInfo.businessHours" :title="merchantInfo.businessHours"
                :label="merchantInfo.businessHours">
          <template #icon>
            <u-icon name="clock" size="20" color="#FF2442"></u-icon>
          </template>
        </u-cell>
      </u-cell-group>
    </view>

    <!-- 标签栏 -->
    <view class="sticky-tab">
      <u-tabs :list="tabs" :current="currentTab" @change="tabChange" 
              activeColor="#FF2442" inactiveColor="#666" barWidth="40" 
              fontSize="14" :barStyle="{height: '3px'}" lineColor="#FF2442"></u-tabs>
    </view>

    <!-- 内容区域 -->
    <view class="content-area">
      <!-- 商品分类 -->
      <u-scroll-list v-if="currentTab === 0 && categories.length > 1" 
                    :indicator="false" height="80">
        <view class="category-list">
          <u-tag v-for="(category, index) in categories" :key="index" 
                :text="category.name" size="medium"
                :type="currentCategory === category.id ? 'error' : 'info'"
                :plain="currentCategory !== category.id" 
                :customStyle="{marginRight: '20rpx'}"
                @click="switchCategory(category.id)"></u-tag>
        </view>
      </u-scroll-list>

      <!-- 商品列表 -->
     <u-grid v-if="currentTab === 0" :col="2" :border="false">
       <u-grid-item v-for="(item, index) in filteredProducts" :key="index" 
                   @click="viewProductDetail(item)">
         <!-- 使用 u-cell-group 创建卡片容器 -->
         <u-cell-group :border="false" class="product-card">
           <!-- 商品图片 -->
           <view class="card-body">
             <u--image :src="item.cover" width="100%" height="320rpx" 
                       mode="aspectFill" radius="8" lazyLoad></u--image>
             
             <!-- 商品标签 -->
             <view class="tags-container u-m-t-10" v-if="item.tags && item.tags.length">
               <u-tag v-for="(tag, tagIndex) in item.tags.slice(0, 2)" :key="tagIndex" 
                     :text="tag" size="mini" type="error" shape="circle" plain
                     :customStyle="{marginRight: '8rpx'}"></u-tag>
             </view>
           </view>
           
           <!-- 商品信息 -->
           <view class="card-foot u-p-t-10">
             <u-text :text="item.name" size="14" lines="2" class="u-line-2 u-font-bold"></u-text>
             
             <view class="u-flex u-row-between u-col-center u-m-t-10">
               <view class="u-flex u-col-center">
                 <u-text :text="'¥'" type="error" size="12" class="u-m-r-4"></u-text>
                 <u-text :text="item.price" type="error" size="16"></u-text>
               </view>
               <u-text :text="'已售' + item.sales" size="12" color="#999"></u-text>
             </view>
           </view>
         </u-cell-group>
       </u-grid-item>
     </u-grid>
    </view>
  </view>
</template>

<script>
	export default {
		data() {
			return {
				currentTab: 0,
				currentCategory: 'all', // 当前选中的分类
				tabs: [{
						name: '商品'
					},
					{
						name: '商户动态'
					}
				],
				// 商品分类数据
				categories: [{
						id: 'all',
						name: '全部'
					},
					{
						id: 'jewelry',
						name: '珠宝首饰'
					},
					{
						id: 'watch',
						name: '名表'
					},
					{
						id: 'accessory',
						name: '配饰'
					},
					{
						id: 'custom',
						name: '定制服务'
					}
				],
				merchantInfo: {
					logo: '/static/logo.png',
					name: '六福珠宝旗舰店',
					tags: ['品牌直营', '20年老店', '正品保障', '免费清洗', '终身保养'],
					description: '六福集团成立于1991年，于1997年5月在香港联合交易所有限公司主板上市，是中国香港及中国内地主要珠宝零售商之一。',
					swiperImages: [
						'/static/swiper-background-1.png',
						'/static/swiper-background-2.png',
						'/static/swiper-background-3.png'
					],
					productCount: 128,
					serviceCount: 12,
					rating: '4.9',
					address: '北京市朝阳区建国路87号北京SKP购物中心B1层',
					businessHours: '周一至周日 10:00-22:00',
					phone: '010-12345678'
				},
				// 商品数据（添加了分类信息）
				productList: [{
						id: 1,
						cover: '/static/product/00d68809-047c-4764-af68-d62782e8dbfa_large.jpg',
						name: '18K金钻石项链 奢华定制款',
						price: '5999',
						sales: 2568,
						category: 'jewelry',
						categoryName: '珠宝首饰',
						tags: ['新品', '畅销']
					},
					{
						id: 2,
						cover: '/static/product/0ab16799-a0de-407c-9e7f-5193b3da5ad1_large.jpg',
						name: '足金手镯 经典传承系列',
						price: '12999',
						sales: 1890,
						category: 'jewelry',
						categoryName: '珠宝首饰',
						tags: ['经典', '足金']
					},
					{
						id: 3,
						cover: '/static/product/0ace2500-c366-476d-81f0-46d4bfcdd54d_large.jpg',
						name: '珍珠耳环 优雅气质款',
						price: '899',
						sales: 3256,
						category: 'jewelry',
						categoryName: '珠宝首饰',
						tags: ['珍珠', '优雅']
					},
					{
						id: 4,
						cover: '/static/product/0d122133-ef9b-46af-b409-6b2082a336d7_large.jpg',
						name: '情侣对戒 永恒之约系列',
						price: '3999',
						sales: 4521,
						category: 'jewelry',
						categoryName: '珠宝首饰',
						tags: ['情侣', '对戒']
					},
					{
						id: 5,
						cover: '/static/product/0df91bfe-d34f-42e3-9ffa-ab16495244cd_large.jpg',
						name: '翡翠吊坠 吉祥如意款',
						price: '2599',
						sales: 2890,
						category: 'jewelry',
						categoryName: '珠宝首饰',
						tags: ['翡翠', '吉祥']
					},
					{
						id: 6,
						cover: 'https://via.placeholder.com/300x400/FF6347/000000',
						name: '瑞士机械表 商务精英款',
						price: '19999',
						sales: 1562,
						category: 'watch',
						categoryName: '名表',
						tags: ['瑞士', '机械']
					},
					{
						id: 7,
						cover: 'https://via.placeholder.com/300x400/4682B4/000000',
						name: '真皮手表带 定制服务',
						price: '299',
						sales: 890,
						category: 'accessory',
						categoryName: '配饰',
						tags: ['定制', '真皮']
					},
					{
						id: 8,
						cover: 'https://via.placeholder.com/300x400/9370DB/000000',
						name: '珠宝定制设计服务',
						price: '500',
						sales: 321,
						category: 'custom',
						categoryName: '定制服务',
						tags: ['设计', '定制']
					}
				],
				newsList: [{
						id: 1,
						title: '六福珠宝2023秋季新品上市',
						content: '本季新品以"秋日私语"为主题，融合自然元素与现代设计，推出多款独具匠心的珠宝作品...',
						time: '2023-09-15',
						views: 1256,
						image: 'https://via.placeholder.com/300x200/FFD700/000000'
					},
					{
						id: 2,
						title: '国庆黄金周优惠活动',
						content: '10月1日-10月7日，全场商品8折优惠，满5000元赠送精美礼品，更有限量款首发...',
						time: '2023-09-20',
						views: 986
					},
					{
						id: 3,
						title: '珠宝保养小知识',
						content: '如何正确保养您的珠宝首饰，延长其使用寿命，保持光泽如新...',
						time: '2023-08-25',
						views: 642,
						image: 'https://via.placeholder.com/300x200/90EE90/000000'
					}
				]
			}
		},
		computed: {
			statsData() {
				return [{
						value: this.merchantInfo.productCount,
						label: '商品'
					},
					{
						value: this.merchantInfo.serviceCount,
						label: '服务'
					},
					{
						value: this.merchantInfo.rating || '5.0',
						label: '评分'
					}
				]
			},
			// 根据当前分类筛选商品
			filteredProducts() {
				if (this.currentCategory === 'all') {
					return this.productList;
				}
				return this.productList.filter(item => item.category === this.currentCategory);
			}
		},
		methods: {
			tabChange(index) {
				this.currentTab = index
				uni.pageScrollTo({
					scrollTop: 0,
					duration: 300
				})
			},
			// 切换商品分类
			switchCategory(categoryId) {
				this.currentCategory = categoryId;
				// 可以添加滚动到顶部的操作
				uni.pageScrollTo({
					scrollTop: 0,
					duration: 200
				});
			},

			viewProductDetail(product) {
				uni.navigateTo({
					url: `/pages/product/detail?id=${product.id}`,
					animationType: 'slide-in-right',
					animationDuration: 300
				})
			},
			contactMerchant() {
				uni.showActionSheet({
					itemList: ['拨打电话: ' + this.merchantInfo.phone, '复制电话号码'],
					success: (res) => {
						if (res.tapIndex === 0) {
							uni.makePhoneCall({
								phoneNumber: this.merchantInfo.phone
							})
						} else {
							uni.setClipboardData({
								data: this.merchantInfo.phone,
								success: () => {
									uni.showToast({
										title: '电话号码已复制',
										icon: 'success'
									})
								}
							})
						}
					}
				})
			},
			copyAddress() {
				if (this.merchantInfo.address) {
					uni.setClipboardData({
						data: this.merchantInfo.address,
						success: () => {
							uni.showToast({
								title: '地址已复制',
								icon: 'success'
							})
						}
					})
				}
			}
		}
	}
</script>

<style lang="scss" scoped>
	.profile-container {
		background-color: #f8f8f8;
		padding-bottom: 30rpx;
	}

	/* 商户卡片样式 */
	.merchant-card {
		position: relative;
		margin-bottom: 20rpx;
		background-color: #fff;
		border-radius: 16rpx;
		overflow: hidden;
		box-shadow: 0 2rpx 12rpx rgba(0, 0, 0, 0.03);

		.swiper-container {
			position: relative;
			height: 300rpx;
			overflow: hidden;

			.bg-swiper {
				height: 100%;

				.swiper-image {
					width: 100%;
					height: 100%;
				}
			}
		}

		.merchant-header {
			position: relative;
			padding: 0 30rpx 30rpx;
			margin-top: -45rpx;

			.logo-wrapper {
				display: flex;
				justify-content: center;
				margin-bottom: 20rpx;
			}

			.base-info {
				display: flex;
				flex-direction: column;
				align-items: center;
				margin-bottom: 20rpx;

				.merchant-name {
					margin-bottom: 12rpx;
				}

				.tags-scroll {
					width: 100%;
					margin-bottom: 12rpx;
					white-space: nowrap;

					.tags-container {
						display: inline-flex;
						padding: 8rpx 0;

						.tag-item {
							margin-right: 12rpx;
							flex-shrink: 0;
						}
					}
				}

				.description {
					text-align: center;
					padding: 0 30rpx;
					margin-bottom: 10rpx;
				}
			}

			.action-buttons {
				position: absolute;
				top: 10rpx;
				right: 30rpx;
			}
		}

		.stats {
			display: flex;
			justify-content: space-around;
			padding: 24rpx 0;
			border-top: 1rpx solid #f5f5f5;

			.stat-item {
				display: flex;
				flex-direction: column;
				align-items: center;
				min-width: 100rpx;

				.stat-num {
					margin-bottom: 6rpx;
				}
			}
		}

		.business-info {
			padding: 20rpx 30rpx;
			border-top: 1rpx solid #f5f5f5;

			.info-item {
				display: flex;
				align-items: center;
				padding: 12rpx 0;

				.info-text {
					flex: 1;
					margin: 0 12rpx;
				}
			}
		}
	}

	/* 粘性标签栏 */
	.sticky-tab {
		position: sticky;
		top: 0;
		z-index: 99;
		background-color: #fff;
		padding: 20rpx 0;
		box-shadow: 0 2rpx 10rpx rgba(0, 0, 0, 0.03);
	}

	/* 内容区域 */
	.content-area {
		padding: 0 20rpx;

		/* 商品分类标签栏 */
		.category-tabs {
			background-color: #fff;
			padding: 20rpx 0 10rpx;
			margin-bottom: 20rpx;
			box-shadow: 0 2rpx 10rpx rgba(0, 0, 0, 0.03);

			.category-scroll {
				width: 100%;
				white-space: nowrap;

				.category-list {
					display: inline-flex;
					padding: 0 20rpx;

					.category-item {
						flex-shrink: 0;
						padding: 10rpx 24rpx;
						margin-right: 16rpx;
						border-radius: 30rpx;
						background-color: #f5f5f5;
						transition: all 0.2s;

						&.active {
							background-color: #FF2442;
							color: #fff;
						}
					}
				}
			}
		}

		/* 商品网格 */
		.product-grid {
			display: grid;
			grid-template-columns: repeat(2, 1fr);
			gap: 20rpx;
			padding: 0 20rpx;

			.product-item {
				background-color: #fff;
				border-radius: 12rpx;
				overflow: hidden;
				transition: all 0.2s;
				display: flex;
				flex-direction: column;

				.image-wrapper {
					position: relative;
					width: 100%;
					height: 320rpx;
					overflow: hidden;

					.product-image {
						width: 100%;
						height: 100%;
						transition: transform 0.3s;
					}

					.product-tags {
						position: absolute;
						bottom: 10rpx;
						left: 10rpx;
						display: flex;
						flex-wrap: wrap;

						.tag-item {
							margin-right: 8rpx;
							margin-bottom: 8rpx;
						}
					}
				}

				.product-info {
					padding: 16rpx;
					flex: 1;
					display: flex;
					flex-direction: column;

					.product-title {
						margin-bottom: 10rpx;
						flex-shrink: 0;
					}

					.product-meta {
						display: flex;
						justify-content: space-between;
						align-items: center;
						margin-top: auto;
						padding-top: 10rpx;
					}

					.product-footer {
						margin-top: 10rpx;

						.category-label {
							display: inline-block;
							border-radius: 20rpx;
						}
					}
				}

				&.product-hover {
					box-shadow: 0 4rpx 12rpx rgba(255, 36, 66, 0.1);

					.product-image {
						transform: scale(1.03);
					}
				}
			}
		}

		/* 动态列表 */
		.news-list {
			.news-item {
				display: flex;
				background-color: #fff;
				border-radius: 12rpx;
				overflow: hidden;
				margin-bottom: 20rpx;
				padding: 24rpx;
				transition: all 0.2s;

				.news-content {
					flex: 1;
					display: flex;
					flex-direction: column;

					.news-title {
						margin-bottom: 12rpx;
					}

					.news-desc {
						margin-bottom: 16rpx;
						line-height: 1.4;
					}

					.news-footer {
						display: flex;
						justify-content: space-between;
						align-items: center;

						.view-info {
							display: flex;
							align-items: center;

							.view-count {
								margin-left: 6rpx;
							}
						}
					}
				}

				.news-image {
					width: 200rpx;
					height: 140rpx;
					margin-left: 20rpx;
					border-radius: 8rpx;
				}

				&.news-hover {
					box-shadow: 0 4rpx 12rpx rgba(0, 0, 0, 0.05);
				}
			}
		}
	}
</style>