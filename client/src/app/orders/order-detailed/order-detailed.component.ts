import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IOrder } from 'src/app/shared/models/order';
import { BreadcrumbService } from 'xng-breadcrumb';
import { OrdersService } from '../orders.service';

@Component({
  selector: 'app-order-detailed',
  templateUrl: './order-detailed.component.html',
  styleUrls: ['./order-detailed.component.scss']
})
export class OrderDetailedComponent implements OnInit {
  order: IOrder;

  constructor(private ordersService: OrdersService, 
              private bcService: BreadcrumbService,
              private activatedRoute: ActivatedRoute) {
    this.bcService.set("@OrderDetailed", " ");
  }

  ngOnInit(): void {
    this.loadOrder();
  }

  loadOrder() {
    this.ordersService.getOrderDetailed(+this.activatedRoute.snapshot.paramMap.get('id')).subscribe({
      next: (order: IOrder) => {
        this.order = order;
        this.bcService.set('@OrderDetailed', `Order# ${order.id} - ${order.status}`);
      },
      error: (e) => console.log(e)
    });
  }

}
